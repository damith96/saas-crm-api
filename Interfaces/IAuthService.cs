using VertexCRM.DTOs.Auth;

namespace VertexCRM.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO request);

    Task<AuthResponseDTO> LoginUser(LoginRequestDTO request);
}
