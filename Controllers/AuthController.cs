using Microsoft.AspNetCore.Mvc;
using VertexCRM.Constants;
using VertexCRM.DTOs.Auth;
using VertexCRM.DTOs.Common;
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
            var result = await _authService.RegisterUser(request);

            return StatusCode(201, new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "User registered successfully.", result));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new CommonResponseDTO(ResponseConstants.StatusMessages.FAILED, ex.Message));
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        try
        {
            var result = await _authService.LoginUser(request);

            return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Login successfully.", result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new CommonResponseDTO(ResponseConstants.StatusMessages.FAILED, ex.Message));
        }
    }
}
