using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertexCRM.Constants;
using VertexCRM.DTOs.Common;
using VertexCRM.DTOs.CrmTask;
using VertexCRM.Interfaces;

namespace VertexCRM.Controllers;

[Authorize]
[ApiController]
[Route("api/companies/{companyId:int}/taksks")]
public class CrmTasksController : ControllerBase
{
    private readonly ICrmTaskService _crmTaskService;

    public CrmTasksController(ICrmTaskService crmTaskService)
    {
        _crmTaskService = crmTaskService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCrmTask(int companyId, CreateCrmTaskRequestDTO request)
    {
        var result = await _crmTaskService.CreateCrmTask(companyId, request);

        return StatusCode(201, new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Task created successfully.", result));
    }

    [HttpGet]
    public async Task<IActionResult> GetCrmTasks(int companyId)
    {
        var result = await _crmTaskService.GetCrmTasks(companyId);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Tasks received successfully.", result));
    }

    [HttpGet("{taskId:int}")]
    public async Task<IActionResult> GetCrmTaskById(int companyId, int taskId)
    {
        var result = await _crmTaskService.GetCrmTaskById(companyId, taskId);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Task received successfully.", result));
    }

    [HttpPut("{taskId:int}")]
    public async Task<IActionResult> UpdateCrmTask(int companyId, int taskId, UpdateCrmTaskRequestDTO request)
    {
        var result = await _crmTaskService.UpdateCrmTask(companyId, taskId, request);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Task updated successfully.", result));
    }

    [HttpDelete("{taskId:int}")]
    public async Task<IActionResult> DeleteCrmTask(int companyId, int taskId)
    {
        await _crmTaskService.DeleteCrmTask(companyId, taskId);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Task deleted successfully."));
    }
}