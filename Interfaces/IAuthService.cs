using VertexCRM.DTOs.Auth;

namespace VertexCRM.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);
}
