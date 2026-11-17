using VertexCRM.Constants;

namespace VertexCRM.Entities;

public sealed class Deal : BaseEntity
{
    public int CompanyId { get; set; }
    public int? ContactId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal? Amount { get; set; }
    public string? CurrencyCode { get; set; }

    public DealStage Stage { get; set; } = DealStage.New;

    public DateOnly? ExpectedCloseDate { get; set; }
    public DateTimeOffset? ClosedOn { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedOn { get; set; }
    public int? DeletedByUserId { get; set; }

    public Company Company { get; set; } = null!;
    public Contact? Contact { get; set; }
    public ICollection<CrmTask> Tasks { get; set; } = [];
    public ICollection<Activity> Activities { get; set; } = [];
}