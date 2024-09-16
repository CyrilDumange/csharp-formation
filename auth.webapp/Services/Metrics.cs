using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace auth.webapp.Services;

public static class MetricsManager
{
  private static Meter app = new("auth.webapp", "1.0");

  public static Counter<int> TokenCreation = app.CreateCounter<int>("token_creation");

  public static readonly ActivitySource SpanPopper = new("auth_webapp.DistributedTracing");
  public static TracerProvider tracerProvider;

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


    tracerProvider = Sdk.CreateTracerProviderBuilder()
      .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("auth_webapp"))
      .AddSource("auth_webapp.DistributedTracing")
      .AddConsoleExporter()
      .AddOtlpExporter(options =>
    {
      options.Endpoint = new Uri("http://localhost:4317");
      options.Protocol = OtlpExportProtocol.Grpc;
    })
      .Build();

  }
}
