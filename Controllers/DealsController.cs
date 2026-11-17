
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertexCRM.Interfaces;
using VertexCRM.Constants;
using VertexCRM.DTOs;
using VertexCRM.DTOs.Deals;
using VertexCRM.DTOs.Deal;
using VertexCRM.DTOs.Common;

namespace VertexCRM.Controllers;

[Authorize]
[ApiController]
[Route("api/companies/{companyId:int}/deals")]
public class DealsController : ControllerBase
{
    private readonly IDealService _dealService;

    public DealsController(IDealService dealService)
    {
        _dealService = dealService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDeal(
        int companyId,
        CreateDealRequestDTO request)
    {
        var result = await _dealService.CreateDeal(companyId, request);

        return StatusCode(
            201,
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Deal created successfully.",
                result));
    }

    [HttpGet]
    public async Task<IActionResult> GetDeals(int companyId)
    {
        var result = await _dealService.GetDeals(companyId);

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Deals retrieved successfully.",
                result));
    }

    [HttpGet("{dealId:int}")]
    public async Task<IActionResult> GetDealById(
        int companyId,
        int dealId)
    {
        var result = await _dealService.GetDealById(companyId, dealId);

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Deal retrieved successfully.",
                result));
    }

    [HttpPut("{dealId:int}")]
    public async Task<IActionResult> UpdateDeal(
        int companyId,
        int dealId,
        UpdateDealRequestDTO request)
    {
        var result = await _dealService.UpdateDeal(
            companyId,
            dealId,
            request);

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Deal updated successfully.",
                result));
    }

    [HttpDelete("{dealId:int}")]
    public async Task<IActionResult> DeleteDeal(
        int companyId,
        int dealId)
    {
        await _dealService.DeleteDeal(companyId, dealId);

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Deal deleted successfully."));
    }
}
