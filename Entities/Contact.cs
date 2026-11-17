namespace VertexCRM.Entities;

public sealed class Contact : BaseEntity
{
    public int CompanyId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? JobTitle { get; set; }
    public string? Description { get; set; }
    public bool IsDeleted { get; set; } = false;

    public Company Company { get; set; } = null!;
    public ICollection<Deal> Deals { get; set; } = [];
    public ICollection<CrmTask> Tasks { get; set; } = [];
    public ICollection<Activity> Activities { get; set; } = [];
}