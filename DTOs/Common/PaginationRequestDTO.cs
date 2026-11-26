using System.ComponentModel.DataAnnotations;

namespace VertexCRM.DTOs.Common;

public sealed class PaginationRequestDTO
{
    private const int MaxPageSize = 100;
    private int _pagesize = 10;
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    public int PageSize
    {
        get => _pagesize;
        set => _pagesize = Math.Clamp(value, 1, MaxPageSize);
    }
    [MaxLength(100)]
    public string? Search { get; set; }
    public string? Status { get; set; }
    public bool IncludeArchieved { get; set; } = false;
}

// public class PaginationQueryDTO<T>
// {
//     public required IEnumerable<T> Results { get; set; }
//     public int PageNumber { get; set; }
//     public int PageSize { get; set; }
//     public int TotalRecords { get; set; }
//     public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
//     /// <summary>Opaque cursor for the next page. Present only when cursor-based pagination is used.</summary>
//     public DateTime? NextCursor { get; set; }
// }