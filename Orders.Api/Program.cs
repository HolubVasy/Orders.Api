using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Orders.Api.Observability;

namespace Orders.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();

                logging.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(
                                serviceName: OrderTelemetry.ServiceName,
                                serviceVersion: OrderTelemetry.ServiceVersion));

                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;

                    options.AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri("http://localhost:4317");
                    });
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}