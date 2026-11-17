using System.ComponentModel.DataAnnotations;
using VertexCRM.Constants;

namespace VertexCRM.DTOs.CrmTask;

public sealed class UpdateCrmTaskRequestDTO
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? ContactId { get; set; }

    public int? DealId { get; set; }

    [EnumDataType(typeof(CrmTaskStatus))]
    public CrmTaskStatus Status { get; set; }

    [EnumDataType(typeof(CrmTaskPriority))]
    public CrmTaskPriority Priority { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public int? AssignedToUserId { get; set; }
}