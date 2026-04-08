using GymErp.Common;
using GymErp.Domain.Subscriptions.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace GymErp.Tenant;

public class SubscriptionsDbContextFactory(
    IConfiguration configuration,
    IServiceBus serviceBus,
    ILogger<SubscriptionsDbContextFactory> logger)
    : IEfDbContextFactory<SubscriptionsDbContext>
{
    public async Task<SubscriptionsDbContext> CriarAsync(string codigoTenant)
    {
        var postgresTenantStringConnection = new PostgresTenantStringConnection(configuration);
        var connectionString = postgresTenantStringConnection.GetTenantsConfigStringConnection();
        var connInfo = new NpgsqlConnectionStringBuilder(connectionString);
        logger.LogInformation(
            "TenantDbFactory connection info host={Host} database={Database} username={Username} hasPassword={HasPassword}",
            connInfo.Host,
            connInfo.Database,
            connInfo.Username,
            !string.IsNullOrWhiteSpace(connInfo.Password));

        var options = new DbContextOptionsBuilder<SubscriptionsDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        var context = new SubscriptionsDbContext(options, serviceBus);

        // Garanti schema inicial (workshop/dev). Sem migrations, a primeira escrita falha sem a tabela.
        try
        {
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "TenantDbFactory EnsureCreated failed for tenant={Tenant}", codigoTenant);
            throw;
        }
        return context;
    }
}
