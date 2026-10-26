using System.ComponentModel.DataAnnotations;

namespace VertexCRM.DTOs.Auth;

public class ForgotPasswordRequestDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
