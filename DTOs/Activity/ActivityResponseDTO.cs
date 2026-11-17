using System.ComponentModel.DataAnnotations;
using VertexCRM.Constants;

namespace VertexCRM.DTOs.Activity;

public sealed class ActivityResponseDTO
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int? ContactId { get; set; }
    public int? DealId { get; set; }
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTimeOffset ActivityDate { get; set; }
    public int CreatedByUserId { get; set; }
}
