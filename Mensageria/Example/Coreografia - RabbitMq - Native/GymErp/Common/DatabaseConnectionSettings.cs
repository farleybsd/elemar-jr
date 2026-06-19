namespace GymErp.Common;

public sealed class DatabaseConnectionSettings
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string User { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public bool DisableSsl { get; init; }
    public bool Pooling { get; init; }
    public int MaxPoolSize { get; init; }
    public int MinPoolSize { get; init; }
    public int Timeout { get; init; }
    public int ConnectionIdleLifetime { get; init; }
    public bool Multiplexing { get; init; }
}
