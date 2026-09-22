namespace VertexCRM.DTOs.Auth;

public class AuthResponseDTO
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;
}
