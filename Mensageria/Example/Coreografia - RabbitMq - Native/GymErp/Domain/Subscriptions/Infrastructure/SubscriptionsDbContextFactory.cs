using GymErp.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GymErp.Domain.Subscriptions.Infrastructure;

public sealed class SubscriptionsDbContextFactory : IDesignTimeDbContextFactory<SubscriptionsDbContext>
{
    public SubscriptionsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var databaseConnection = configuration
            .GetSection("DatabaseConnection")
            .Get<DatabaseConnectionSettings>()
            ?? throw new InvalidOperationException("DatabaseConnection não configurado.");

        var connectionString = PostgresConnectionStringBuilder.Build(databaseConnection);

        var options = new DbContextOptionsBuilder<SubscriptionsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new SubscriptionsDbContext(options, new NullServiceBus());
    }

    private sealed class NullServiceBus : IServiceBus
    {
        public Task PublishAsync(object message)
        {
            return Task.CompletedTask;
        }
    }
}
