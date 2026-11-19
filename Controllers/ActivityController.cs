using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertexCRM.Constants;
using VertexCRM.DTOs.Common;
using VertexCRM.DTOs.Activity;
using VertexCRM.Interfaces;
using VertexCRM.Attributes;

namespace VertexCRM.Controllers;

[Authorize]
[ApiController]
[CompanyAccess]
[Route("api/companies/{companyId:int}/activities")]
public class ActivityController : ControllerBase
{
    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivity(int companyId, CreateActivityRequestDTO request)
    {
        var result = await _activityService.CreateActivity(companyId, request);

        return StatusCode(201, new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Activity created successfully.", result));
    }

    [HttpGet]
    public async Task<IActionResult> GetActivities(int companyId)
    {
        var result = await _activityService.GetActivities(companyId);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Activities received successfully.", result));
    }

    [HttpGet("{activityId:int}")]
    public async Task<IActionResult> GetActivityById(int companyId, int activityId)
    {
        var result = await _activityService.GetActivityById(companyId, activityId);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Activity received successfully.", result));
    }

    [HttpPut("{activityId:int}")]
    public async Task<IActionResult> UpdateActivity(int companyId, int activityId, UpdateActivityRequestDTO request)
    {
        var result = await _activityService.UpdateActivity(companyId, activityId, request);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Activity updated successfully.", result));
    }

    [HttpDelete("{activityId:int}")]
    public async Task<IActionResult> DeleteActivity(int companyId, int activityId)
    {
        await _activityService.DeleteActivity(companyId, activityId);

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Activity deleted successfully."));
    }
}