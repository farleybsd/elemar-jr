namespace GymErp.Tenant;

public class DefaultTenantHttpAccessor(IHttpContextAccessor httpContextAccessor) : Common.ITenantHttpAccessor
{
    private const string DefaultTenant = "default";
    private const string TenantHeader = "X-Tenant";

    public string Get()
    {
        var tenant = httpContextAccessor.HttpContext?.Request.Headers[TenantHeader].FirstOrDefault();
        return string.IsNullOrWhiteSpace(tenant) ? DefaultTenant : tenant;
    }
}
