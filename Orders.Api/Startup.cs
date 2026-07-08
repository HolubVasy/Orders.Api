using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Orders.Api;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddSingleton<OrderTelemetry>();

        services.AddScoped<OrderService>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddHttpClient<PaymentClient>(client =>
        {
            client.BaseAddress = new Uri("https://httpbin.org");
        });

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: OrderTelemetry.ServiceName,
                    serviceVersion: OrderTelemetry.ServiceVersion);
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(OrderTelemetry.ActivitySourceName)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri("http://localhost:4317");
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(OrderTelemetry.MeterName)
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri("http://localhost:4317");
                    });
            });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}