namespace Orders.Api;

public class PaymentClient
{
    private readonly HttpClient _httpClient;
    private readonly OrderTelemetry _telemetry;

    public PaymentClient(HttpClient httpClient, OrderTelemetry telemetry)
    {
        _httpClient = httpClient;
        _telemetry = telemetry;
    }

    public async Task AuthorizeAsync(Guid orderId, CancellationToken cancellationToken)
    {
        using var activity = _telemetry.ActivitySource.StartActivity(
            "PaymentClient.Authorize",
            System.Diagnostics.ActivityKind.Client);

        activity?.SetTag("order.id", orderId);
        activity?.SetTag("payment.provider", "fake-payment");

        var response = await _httpClient.GetAsync("/status/200", cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}