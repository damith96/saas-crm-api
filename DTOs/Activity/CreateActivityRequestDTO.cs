using System.ComponentModel.DataAnnotations;
using VertexCRM.Constants;

namespace VertexCRM.DTOs.Activity;

// Use nullable value types (e.g., enum?, int?, DateTimeOffset?, bool?) with required
// to ensure clients explicitly provide values instead of silently receiving defaults (deafults through model binding).
public sealed class CreateActivityRequestDTO
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