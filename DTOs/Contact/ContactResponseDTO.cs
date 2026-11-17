namespace VertexCRM.DTOs.Contact;

public sealed class ContactResponseDTO
{
    public int Id { get; set; }
    public int CompanyId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? JobTitle { get; set; }
    public string? Description { get; set; }
}