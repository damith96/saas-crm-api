using VertexCRM.DTOs.Common;
using VertexCRM.DTOs.Company;

namespace VertexCRM.Interfaces;

public interface ICompanyService
{
    Task<List<AllowedCompanyResponseDTO>> GetAllowedCompanies();
    Task<PaginatedResponseDTO<CompanyListResponseDTO>> GetCompanies(PaginationRequestDTO request);
    Task<CompanyListResponseDTO> CreateCompanyAsync(
    CreateCompanyRequestDTO request);
    Task<CompanyListResponseDTO?> GetCompanyByIdAsync(int id);
    Task<CompanyListResponseDTO?> UpdateCompanyAsync(
    int id,
    UpdateCompanyRequestDTO request);
    Task<bool> DeleteCompany(int id);
}