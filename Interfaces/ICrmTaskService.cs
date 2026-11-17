using VertexCRM.DTOs.CrmTask;

namespace VertexCRM.Interfaces;

public interface ICrmTaskService
{
    Task<CrmTaskResponseDTO> CreateCrmTask(
        int companyId,
        CreateCrmTaskRequestDTO request);

    Task<List<CrmTaskResponseDTO>> GetCrmTasks(
        int companyId);

    Task<CrmTaskResponseDTO> GetCrmTaskById(
        int companyId,
        int taskId);

    Task<CrmTaskResponseDTO> UpdateCrmTask(
        int companyId,
        int taskId,
        UpdateCrmTaskRequestDTO request);

    Task DeleteCrmTask(
        int companyId,
        int taskId);
}