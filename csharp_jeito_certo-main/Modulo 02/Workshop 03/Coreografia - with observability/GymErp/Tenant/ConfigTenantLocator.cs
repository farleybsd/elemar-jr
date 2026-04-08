using GymErp.Common;

namespace GymErp.Tenant;

/// <summary>
/// Retorna um único tenant usando a connection string da seção DatabaseConnection (cenário single-tenant / desenvolvimento).
/// </summary>
public class ConfigTenantLocator(PostgresTenantStringConnection postgresTenantStringConnection) : ITenantLocator
{
    private const string DefaultTenantName = "default";

    public Task<GymErpTenant> Get(string identifier, CancellationToken cancellationToken)
    {
        var connectionString = postgresTenantStringConnection.GetTenantsConfigStringConnection();
        var tenant = new GymErpTenant(Guid.Empty, DefaultTenantName, connectionString);
        return Task.FromResult(tenant);
    }

    public Task<IEnumerable<GymErpTenant>> GetAll(CancellationToken cancellationToken)
    {
        var tenant = Get(string.Empty, cancellationToken).GetAwaiter().GetResult();
        return Task.FromResult((IEnumerable<GymErpTenant>)new[] { tenant });
    }
}
