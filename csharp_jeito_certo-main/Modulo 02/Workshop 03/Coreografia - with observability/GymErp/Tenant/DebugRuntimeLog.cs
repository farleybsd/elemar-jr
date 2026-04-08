using System.Text.Json;
using System.Text;

namespace GymErp.Tenant;

internal static class DebugRuntimeLog
{
    private const string FallbackLogPath = "/.cursor/debug-f52434.log";
    private const string SessionId = "f52434";
    private const string IngestEndpoint = "http://host.docker.internal:7242/ingest/0705c001-7a0a-4abe-bee3-203464aa1c1e";
    private static readonly HttpClient Http = new();

    public static void Write(string runId, string hypothesisId, string location, string message, object data)
    {
        var payload = new
        {
            sessionId = SessionId,
            runId,
            hypothesisId,
            location,
            message,
            data,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        try
        {
            // Prefer HTTP ingest so host-side debug collector owns file creation.
            _ = Task.Run(async () =>
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, IngestEndpoint);
                    request.Headers.TryAddWithoutValidation("X-Debug-Session-Id", SessionId);
                    request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                    await Http.SendAsync(request);
                }
                catch
                {
                    // no-op
                }
            });

            var logPath = Environment.GetEnvironmentVariable("DEBUG_LOG_PATH");
            if (string.IsNullOrWhiteSpace(logPath))
                logPath = FallbackLogPath;
            var logDir = Path.GetDirectoryName(logPath);
            if (!string.IsNullOrWhiteSpace(logDir))
                Directory.CreateDirectory(logDir);
            File.AppendAllText(logPath, JsonSerializer.Serialize(payload) + Environment.NewLine);
        }
        catch
        {
            // no-op: debug logging must not break request flow
        }
    }
}
