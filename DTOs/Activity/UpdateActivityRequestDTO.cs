using System.ComponentModel.DataAnnotations;
using VertexCRM.Constants;

namespace VertexCRM.DTOs.Activity;

public sealed class UpdateActivityRequestDTO
{
    [Required]
    [EnumDataType(typeof(ActivityType))]
    public ActivityType? Type { get; set; }

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [MaxLength(3000)]
    public string? Description { get; set; }
    public int? ContactId { get; set; }
    public int? DealId { get; set; }

    [Required]
    public DateTimeOffset? ActivityDate { get; set; }
}