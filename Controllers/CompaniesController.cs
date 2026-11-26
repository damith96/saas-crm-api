
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertexCRM.Interfaces;
using VertexCRM.Constants;
using VertexCRM.DTOs.Common;

namespace VertexCRM.Controllers;

[Authorize]
[ApiController]
[Route("api/companies")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet("allowed")]
    public async Task<IActionResult> GetAllowedCompanies()
    {
        var result = await _companyService.GetAllowedCompanies();

        return Ok(new CommonResponseDTO(
            ResponseConstants.StatusMessages.SUCCESS,
            "Allowed companies retrieved successfully.",
            result
        ));
    }

    [HttpGet]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> GetCompanies([FromQuery] PaginationRequestDTO request)
    {
        var result = await _companyService.GetCompanies(request);

        return Ok(new CommonResponseDTO(
            ResponseConstants.StatusMessages.SUCCESS,
            "Companies retrieved successfully.",
            result
        ));
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateContact(
    //     int companyId,
    //     CreateContactRequestDTO request)
    // {
    //     var result = await _contactService.CreateContact(companyId, request);

    //     return StatusCode(
    //         201,
    //         new CommonResponseDTO(
    //             ResponseConstants.StatusMessages.SUCCESS,
    //             "Contact created successfully.",
    //             result
    //         )
    //     );
    // }

    // [HttpGet]
    // public async Task<IActionResult> GetContacts(int companyId)
    // {
    //     var result = await _contactService.GetContacts(companyId);

    //     return Ok(
    //         new CommonResponseDTO(
    //             ResponseConstants.StatusMessages.SUCCESS,
    //             "Contacts retrieved successfully.",
    //             result
    //         )
    //     );
    // }

    // [HttpGet("{contactId:int}")]
    // public async Task<IActionResult> GetContactById(
    //     int companyId,
    //     int contactId)
    // {
    //     var result = await _contactService.GetContactById(
    //         companyId,
    //         contactId
    //     );

    //     return Ok(
    //         new CommonResponseDTO(
    //             ResponseConstants.StatusMessages.SUCCESS,
    //             "Contact retrieved successfully.",
    //             result
    //         )
    //     );
    // }

    // [HttpPut("{contactId:int}")]
    // public async Task<IActionResult> UpdateContact(
    //     int companyId,
    //     int contactId,
    //     UpdateContactRequestDTO request)
    // {
    //     var result = await _contactService.UpdateContact(
    //         companyId,
    //         contactId,
    //         request
    //     );

    //     return Ok(
    //         new CommonResponseDTO(
    //             ResponseConstants.StatusMessages.SUCCESS,
    //             "Contact updated successfully.",
    //             result
    //         )
    //     );
    // }

    // [HttpDelete("{contactId:int}")]
    // public async Task<IActionResult> DeleteContact(
    //     int companyId,
    //     int contactId)
    // {
    //     await _contactService.DeleteContact(companyId, contactId);

    //     return Ok(
    //         new CommonResponseDTO(
    //             ResponseConstants.StatusMessages.SUCCESS,
    //             "Contact deleted successfully."
    //         )
    //     );
    // }

    [HttpPost]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> CreateCompany(
    [FromBody] CreateCompanyRequestDTO request)
    {
        var result = await _companyService.CreateCompanyAsync(request);

        return CreatedAtAction(
            nameof(GetCompanyById),
            new { id = result.Id },
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Company created successfully.",
                result
            )
        );
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> GetCompanyById(int id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);

        if (company is null)
        {
            return NotFound(new CommonResponseDTO(
                ResponseConstants.StatusMessages.FAILED,
                "Company not found.",
                null
            ));
        }

        return Ok(new CommonResponseDTO(
            ResponseConstants.StatusMessages.SUCCESS,
            "Company retrieved successfully.",
            company
        ));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> UpdateCompany(
    int id,
    [FromBody] UpdateCompanyRequestDTO request)
    {
        var result = await _companyService.UpdateCompanyAsync(id, request);

        if (result is null)
        {
            return NotFound(new CommonResponseDTO(
                ResponseConstants.StatusMessages.FAILED,
                "Company not found.",
                null
            ));
        }

        return Ok(new CommonResponseDTO(
            ResponseConstants.StatusMessages.SUCCESS,
            "Company updated successfully.",
            result
        ));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "TenantAdmin")]
    public async Task<IActionResult> DeleteCompany(
    int id)
    {
        var deleted = await _companyService.DeleteCompany(id);

        if (!deleted)
        {
            return NotFound(new CommonResponseDTO(
                ResponseConstants.StatusMessages.FAILED,
                "Company not found.",
                null
            ));
        }

        return Ok(new CommonResponseDTO(
            ResponseConstants.StatusMessages.SUCCESS,
            "Company deleted successfully.",
            null
        ));
    }
}
