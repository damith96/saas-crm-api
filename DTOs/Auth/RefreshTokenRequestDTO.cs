using System.ComponentModel.DataAnnotations;

namespace VertexCRM.DTOs.Auth;

public class RefreshTokenRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
