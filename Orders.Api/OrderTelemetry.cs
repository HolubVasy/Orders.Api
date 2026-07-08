using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Orders.Api;

public sealed class OrderTelemetry : IDisposable
{
    public const string ServiceName = "orders-api";
    public const string ServiceVersion = "1.0.0";

    public const string ActivitySourceName = ServiceName;
    public const string MeterName = ServiceName;

    public ActivitySource ActivitySource { get; }
    public Meter Meter { get; }

    public Counter<long> OrdersCreated { get; }
    public Counter<long> OrdersFailed { get; }
    public Histogram<double> OrderProcessingDurationMs { get; }

    public OrderTelemetry()
    {
        ActivitySource = new ActivitySource(ActivitySourceName, ServiceVersion);
        Meter = new Meter(MeterName, ServiceVersion);

        OrdersCreated = Meter.CreateCounter<long>("orders.created");
        OrdersFailed = Meter.CreateCounter<long>("orders.failed");

        OrderProcessingDurationMs = Meter.CreateHistogram<double>(
            "order.processing.duration.ms");
    }

    public void Dispose()
    {
        ActivitySource.Dispose();
        Meter.Dispose();
    }
}