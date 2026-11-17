using VertexCRM.Constants;
using TaskStatus = VertexCRM.Constants;

namespace VertexCRM.Entities;

public sealed class CrmTask : BaseEntity
{
    public int CompanyId { get; set; }

    public int? ContactId { get; set; }
    public int? DealId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public CrmTaskStatus Status { get; set; } = CrmTaskStatus.Pending;
    public CrmTaskPriority Priority { get; set; } = CrmTaskPriority.Medium;

    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletedOn { get; set; }

    public int? AssignedToUserId { get; set; }
    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedOn { get; set; }
    public int? DeletedByUserId { get; set; }

    public Company Company { get; set; } = null!;
    public Contact? Contact { get; set; }
    public Deal? Deal { get; set; }

    public User? AssignedToUser { get; set; }
}