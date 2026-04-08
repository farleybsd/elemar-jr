using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GymErp.Common;

/// <summary>
/// Append NDJSON lines to the debug session log file (path from env DEBUG_LOG_PATH).
/// Used only when debugging; no-op if DEBUG_LOG_PATH is not set.
/// </summary>
internal static class DebugSessionLog
{
    private static readonly string? LogPath = Environment.GetEnvironmentVariable("DEBUG_LOG_PATH");

    public static void Write(string hypothesisId, string location, string message, IReadOnlyDictionary<string, object?>? data = null)
    {
        if (string.IsNullOrEmpty(LogPath)) return;
        try
        {
            var payload = new Dictionary<string, object?>
            {
                ["sessionId"] = "7e6c31",
                ["hypothesisId"] = hypothesisId,
                ["location"] = location,
                ["message"] = message,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            if (data != null)
                foreach (var kv in data)
                    payload[kv.Key] = kv.Value;
            var line = JsonSerializer.Serialize(payload) + "\n";
            File.AppendAllText(LogPath, line);
        }
        catch
        {
            // ignore
        }
    }
}
