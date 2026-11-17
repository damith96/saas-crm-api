using VertexCRM.DTOs.Contact;

namespace VertexCRM.Interfaces;

public interface IContactService
{
    Task<ContactResponseDTO> CreateContact(
        int companyId,
        CreateContactRequestDTO request);

    Task<List<ContactResponseDTO>> GetContacts(int companyId);

    Task<ContactResponseDTO> GetContactById(
        int companyId,
        int contactId);

    Task<ContactResponseDTO> UpdateContact(
        int companyId,
        int contactId,
        UpdateContactRequestDTO request);

    Task DeleteContact(int companyId, int contactId);
}