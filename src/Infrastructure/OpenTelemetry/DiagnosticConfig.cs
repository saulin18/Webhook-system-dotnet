using System.Diagnostics;

namespace Infrastructure.OpenTelemetry;

public static class DiagnosticConfig
{
    public static readonly ActivitySource Source = new("WebhookAspNet");
}
