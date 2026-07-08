using System.Diagnostics;
using OpenTelemetry.Trace;

namespace Orders.Api;

public class OrderService(
    IOrderRepository repository,
    PaymentClient paymentClient,
    OrderTelemetry telemetry,
    ILogger<OrderService> logger)
{
    public async Task<Guid> CreateAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        using var activity = telemetry.ActivitySource.StartActivity(
            "OrderService.CreateOrder",
            ActivityKind.Internal);

        var orderId = Guid.NewGuid();

        activity?.SetTag("order.id", orderId);
        activity?.SetTag("order.operation", "create");

        try
        {
            logger.LogInformation("Creating order {OrderId}", orderId);

            await repository.SaveAsync(orderId, cancellationToken);
            await paymentClient.AuthorizeAsync(orderId, cancellationToken);

            telemetry.OrdersCreated.Add(1);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return orderId;
        }
        catch (Exception ex)
        {
            telemetry.OrdersFailed.Add(1);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);

            logger.LogError(ex, "Order creation failed for {OrderId}", orderId);

            throw;
        }
        finally
        {
            stopwatch.Stop();

            telemetry.OrderProcessingDurationMs.Record(
                stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}