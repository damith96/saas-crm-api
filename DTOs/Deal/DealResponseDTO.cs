using VertexCRM.Constants;

namespace VertexCRM.DTOs.Deals;

public sealed class DealResponseDTO
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int? ContactId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal? Amount { get; set; }

    public string? CurrencyCode { get; set; }

    public DealStage Stage { get; set; }

    public DateOnly? ExpectedCloseDate { get; set; }

    public DateTimeOffset? ClosedOn { get; set; }
}