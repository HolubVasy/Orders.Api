namespace Orders.Api;

public interface IOrderRepository
{
    Task SaveAsync(Guid orderId, CancellationToken cancellationToken);
}

public class OrderRepository : IOrderRepository
{
    private readonly OrderTelemetry _telemetry;

    public OrderRepository(OrderTelemetry telemetry)
    {
        _telemetry = telemetry;
    }

    public async Task SaveAsync(Guid orderId, CancellationToken cancellationToken)
    {
        using var activity = _telemetry.ActivitySource.StartActivity(
            "OrderRepository.Save",
            System.Diagnostics.ActivityKind.Internal);

        activity?.SetTag("order.id", orderId);
        activity?.SetTag("db.system", "fake-db");

        await Task.Delay(80, cancellationToken);
    }
}