using AdvocateERP.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace AdvocateERP.Infrastructure.Services;
public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _tenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        // Try to resolve TenantId from the JWT claim upon service creation
        var tenantIdClaim = _httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == "TenantId")?.Value;

        if (Guid.TryParse(tenantIdClaim, out Guid resolvedId))
        {
            _tenantId = resolvedId;
        }
    }

    public Guid? TenantId => _tenantId;

    public void SetTenantId(Guid tenantId)
    {
        // This method is generally only used internally or during registration
        _tenantId = tenantId;
    }
}