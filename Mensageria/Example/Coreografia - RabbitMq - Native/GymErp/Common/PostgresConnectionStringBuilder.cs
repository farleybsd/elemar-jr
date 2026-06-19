using Npgsql;

namespace GymErp.Common;

public static class PostgresConnectionStringBuilder
{
    public static string Build(DatabaseConnectionSettings settings)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = settings.Host,
            Port = settings.Port,
            Database = settings.DatabaseName,
            Username = settings.User,
            Password = settings.Password,
            SslMode = settings.DisableSsl ? SslMode.Disable : SslMode.Require,
            Pooling = settings.Pooling,
            MaxPoolSize = settings.MaxPoolSize,
            MinPoolSize = settings.MinPoolSize,
            Timeout = settings.Timeout,
            ConnectionIdleLifetime = settings.ConnectionIdleLifetime,
            Multiplexing = settings.Multiplexing
        };

        return builder.ConnectionString;
    }
}
