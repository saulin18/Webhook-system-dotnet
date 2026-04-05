
using System.Diagnostics;


namespace Webhooks.Processing.Messaging;
public static class DiagnosticConfig
{
    public static readonly ActivitySource Source = new("WebhookAspNet");
}
