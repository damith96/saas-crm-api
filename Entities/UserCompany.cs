namespace VertexCRM.Entities;

public sealed class UserCompany : BaseEntity
{
    public int UserId { get; set; }

    public int CompanyId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset AssignedOn { get; set; }
        = DateTimeOffset.UtcNow;

    public int AssignedByUserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;

    public Company Company { get; set; } = null!;
}