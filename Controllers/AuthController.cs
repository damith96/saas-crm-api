using Microsoft.AspNetCore.Mvc;
using VertexCRM.DTOs.Auth;
using VertexCRM.Interfaces;

namespace VertexCRM.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDTO request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
