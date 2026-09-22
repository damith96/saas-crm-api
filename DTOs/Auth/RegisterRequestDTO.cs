using System.ComponentModel.DataAnnotations;

namespace VertexCRM.DTOs.Auth;

public class RegisterRequestDTO
{
    [Required]
    [StringLength(200)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
