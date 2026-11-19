using Microsoft.EntityFrameworkCore;
using VertexCRM.Interfaces;
using VertexCRM.Data;
using VertexCRM.DTOs.Contact;
using VertexCRM.Entities;

namespace VertexCRM.Services;

public sealed class ContactService : IContactService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public ContactService(
        ApplicationDbContext context,
        ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<ContactResponseDTO> CreateContact(
        int companyId,
        CreateContactRequestDTO request)
    {
        var contact = new Contact
        {
            CompanyId = companyId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            JobTitle = request.JobTitle,
            Description = request.Description
        };

        _context.Contacts.Add(contact);

        await _context.SaveChangesAsync();

        return MapToResponse(contact);
    }

    public async Task<List<ContactResponseDTO>> GetContacts(int companyId)
    {
        var contacts = await _context.Contacts
            .AsNoTracking()
            .Where(c => c.CompanyId == companyId)
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync();

        return [.. contacts.Select(MapToResponse)];
    }

    public async Task<ContactResponseDTO> GetContactById(
        int companyId,
        int contactId)
    {
        var contact = await _context.Contacts
           .AsNoTracking()
           .FirstOrDefaultAsync(c =>
               c.Id == contactId &&
               c.CompanyId == companyId);

        if (contact is null)
        {
            throw new KeyNotFoundException("Contact not found.");
        }

        return MapToResponse(contact);
    }

    public async Task<ContactResponseDTO> UpdateContact(
        int companyId,
        int contactId,
        UpdateContactRequestDTO request)
    {
        var contact = await _context.Contacts
            .FirstOrDefaultAsync(c =>
                c.Id == contactId &&
                c.CompanyId == companyId);

        if (contact is null)
        {
            throw new KeyNotFoundException("Contact not found.");
        }

        contact.FirstName = request.FirstName;
        contact.LastName = request.LastName;
        contact.Email = request.Email;
        contact.PhoneNumber = request.PhoneNumber;
        contact.JobTitle = request.JobTitle;
        contact.Description = request.Description;

        await _context.SaveChangesAsync();

        return MapToResponse(contact);
    }

    public async Task DeleteContact(int companyId, int contactId)
    {
        var contact = await _context.Contacts
             .FirstOrDefaultAsync(c =>
                 c.Id == contactId &&
                 c.CompanyId == companyId);

        if (contact is null)
        {
            throw new KeyNotFoundException("Contact not found.");
        }

        contact.IsDeleted = true;
        // contact.DeletedOn = DateTimeOffset.UtcNow;
        // contact.DeletedByUserId = _tenantContext.UserId;

        await _context.SaveChangesAsync();
    }

    private static ContactResponseDTO MapToResponse(Contact contact)
    {
        return new ContactResponseDTO
        {
            Id = contact.Id,
            CompanyId = contact.CompanyId,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            PhoneNumber = contact.PhoneNumber,
            JobTitle = contact.JobTitle,
            Description = contact.Description
        };
    }
}