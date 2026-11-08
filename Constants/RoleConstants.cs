namespace VertexCRM.Constants;

/// <summary>
/// Names of the default roles seeded into the database.
/// Use these constants when looking up or assigning roles to avoid magic strings.
/// </summary>
public static class RoleConstants
{
    /// <summary>
    /// Assigned to every new user on self-registration.
    /// Tenant-scoped — grants standard CRM access within a tenant.
    /// </summary>
    public const string Member = "Member";

    /// <summary>
    /// Tenant owner / administrator role.
    /// Tenant-scoped — grants full control over a tenant workspace.
    /// </summary>
    public const string TenantAdmin = "Tenant Admin";

    /// <summary>
    /// Global platform administrator.
    /// Platform-scoped — grants access across all tenants.
    /// </summary>
    public const string PlatformAdmin = "Platform Admin";

    // Seeded IDs — must match HasData() calls in RoleConfiguration.
    public const int MemberId = 1;
    public const int TenantAdminId = 2;
    public const int PlatformAdminId = 3;
}
