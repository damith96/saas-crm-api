namespace VertexCRM.DTOs.Company;

public sealed class CompanyListResponseDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

}