using VertexCRM.Constants;

namespace VertexCRM.DTOs.CrmTask;

public sealed class CrmTaskResponseDTO
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int? ContactId { get; set; }

    public int? DealId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public CrmTaskStatus Status { get; set; }

    public CrmTaskPriority Priority { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset? CompletedOn { get; set; }

    public int? AssignedToUserId { get; set; }

    public int CreatedByUserId { get; set; }
}