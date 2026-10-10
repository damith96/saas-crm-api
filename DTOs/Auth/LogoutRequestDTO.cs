using System.ComponentModel.DataAnnotations;

namespace VertexCRM.DTOs.Auth;

public class LogoutRequestDTO
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}