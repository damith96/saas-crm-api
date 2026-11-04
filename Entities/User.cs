using VertexCRM.Constants;

namespace VertexCRM.Entities;

public sealed class User : BaseEntity
{
    // User information
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    // Authentication
    public string PasswordHash { get; set; } = string.Empty;

    public UserStatus Status { get; set; }

    public DateTimeOffset? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    // Tenant relationship
    public int? TenantId { get; set; }

    public Tenant? Tenant { get; set; }

    // Role relationship
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;

    // Navigation properties
    public ICollection<UserCompany> UserCompanies { get; set; }
        = [];

    public ICollection<RefreshToken> RefreshTokens { get; set; }
        = [];
}