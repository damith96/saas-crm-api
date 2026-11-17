using System.ComponentModel.DataAnnotations;
using VertexCRM.Constants;

namespace VertexCRM.DTOs.Deals;

public sealed class UpdateDealRequestDTO
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? ContactId { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal? Amount { get; set; }

    [StringLength(3, MinimumLength = 3)]
    public string? CurrencyCode { get; set; }

    [EnumDataType(typeof(DealStage))]
    public DealStage Stage { get; set; }

    public DateOnly? ExpectedCloseDate { get; set; }
}