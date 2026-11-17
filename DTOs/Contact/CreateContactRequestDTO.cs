using System.ComponentModel.DataAnnotations;

namespace VertexCRM.DTOs.Contact;

public sealed class CreateContactRequestDTO
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? PhoneNumber { get; set; }

    [MaxLength(150)]
    public string? JobTitle { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}