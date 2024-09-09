using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace auth.webapp.Services;

public static class MetricsManager
{
    private static Meter app = new("auth.webapp", "1.0");

    public static Counter<int> TokenCreation = app.CreateCounter<int>("token_creation");

    public static void Init()
    {
        _ = Sdk.CreateMeterProviderBuilder()
        .ConfigureResource(resources => resources.AddService("auth_webapp"))
          .AddMeter("auth.webapp")
          .AddConsoleExporter()
          .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://localhost:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        })
          .Build();
    }
}
