namespace VertexCRM.DTOs.Common;

public sealed class PaginatedResponseDTO<T>
{
    public IReadOnlyList<T> Data { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}