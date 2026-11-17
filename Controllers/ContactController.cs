
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertexCRM.Interfaces;
using VertexCRM.Constants;
using VertexCRM.DTOs;
using VertexCRM.DTOs.Contact;
using VertexCRM.DTOs.Common;

namespace VertexCRM.Controllers;

[Authorize]
[ApiController]
[Route("api/companies/{companyId:int}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact(
        int companyId,
        CreateContactRequestDTO request)
    {
        var result = await _contactService.CreateContact(companyId, request);

        return StatusCode(
            201,
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Contact created successfully.",
                result
            )
        );
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts(int companyId)
    {
        var result = await _contactService.GetContacts(companyId);

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Contacts retrieved successfully.",
                result
            )
        );
    }

    [HttpGet("{contactId:int}")]
    public async Task<IActionResult> GetContactById(
        int companyId,
        int contactId)
    {
        var result = await _contactService.GetContactById(
            companyId,
            contactId
        );

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Contact retrieved successfully.",
                result
            )
        );
    }

    [HttpPut("{contactId:int}")]
    public async Task<IActionResult> UpdateContact(
        int companyId,
        int contactId,
        UpdateContactRequestDTO request)
    {
        var result = await _contactService.UpdateContact(
            companyId,
            contactId,
            request
        );

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Contact updated successfully.",
                result
            )
        );
    }

    [HttpDelete("{contactId:int}")]
    public async Task<IActionResult> DeleteContact(
        int companyId,
        int contactId)
    {
        await _contactService.DeleteContact(companyId, contactId);

        return Ok(
            new CommonResponseDTO(
                ResponseConstants.StatusMessages.SUCCESS,
                "Contact deleted successfully."
            )
        );
    }
}
