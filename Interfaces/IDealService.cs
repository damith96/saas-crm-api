using VertexCRM.DTOs.Deal;
using VertexCRM.DTOs.Deals;

namespace VertexCRM.Interfaces;

public interface IDealService
{
    Task<DealResponseDTO> CreateDeal(
        int companyId,
        CreateDealRequestDTO request);

    Task<List<DealResponseDTO>> GetDeals(int companyId);

    Task<DealResponseDTO> GetDealById(
        int companyId,
        int dealId);

    Task<DealResponseDTO> UpdateDeal(
        int companyId,
        int dealId,
        UpdateDealRequestDTO request);

    Task DeleteDeal(int companyId, int dealId);
}