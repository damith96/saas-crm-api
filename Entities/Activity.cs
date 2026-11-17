using VertexCRM.Constants;

namespace VertexCRM.Entities;

public sealed class Activity : BaseEntity
{
    public int CompanyId { get; set; }
    public int? ContactId { get; set; }
    public int? DealId { get; set; }

    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset ActivityDate { get; set; }
    public int CreatedByUserId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedOn { get; set; }
    public int? DeletedByUserId { get; set; }
    public Company Company { get; set; } = null!;
    public Contact? Contact { get; set; }
    public Deal? Deal { get; set; }

}