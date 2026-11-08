using System.Security.Claims;
using VertexCRM.Abstractions;

namespace VertexCRM.Services;

public sealed class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userId, out var id) ? id : null;
        }
    }

    public int? TenantId
    {
        get
        {
            var tenantId = User?.FindFirst("tenant_id")?.Value;

            return int.TryParse(tenantId, out var id) ? id : null;
        }
    }

    public bool IsPlatformAdmin =>
        User?.FindFirst("role_scope")?.Value == "Platform";
}