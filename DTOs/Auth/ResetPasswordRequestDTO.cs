using System.ComponentModel.DataAnnotations;

namespace VertexCRM.DTOs.Auth;

public class ResetPasswordRequestDTO
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
