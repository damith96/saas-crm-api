using VertexCRM.DTOs.Activity;

namespace VertexCRM.Interfaces;

public interface IActivityService
{
    Task<ActivityResponseDTO> CreateActivity(int companyId, CreateActivityRequestDTO request);
    Task<List<ActivityResponseDTO>> GetActivities(int companyId);
    Task<ActivityResponseDTO> GetActivityById(int companyId, int activityId);
    Task<ActivityResponseDTO> UpdateActivity(int companyId, int activityId, UpdateActivityRequestDTO request);
    Task DeleteActivity(int companyId, int activityId);

}