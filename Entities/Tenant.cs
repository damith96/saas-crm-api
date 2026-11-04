using VertexCRM.Constants;

namespace VertexCRM.Entities;

public sealed class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public TenantStatus Status { get; set; } = TenantStatus.Active;

    public int CreatedByUserId { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
        = DateTimeOffset.UtcNow;

    // Navigation properties
    public User CreatedByUser { get; set; } = null!;

    public ICollection<User> Users { get; set; }
        = [];

    public ICollection<Company> Companies { get; set; }
        = [];
}