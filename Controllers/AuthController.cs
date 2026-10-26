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
        var result = await _authService.RegisterUser(request);

        return StatusCode(201, new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "User registered successfully.", result));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDTO request)
    {
        var result = await _authService.LoginUser(request);

        Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>()
                .IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/api/auth"
        });

        result.RefreshToken = string.Empty;

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Login successfully.", result));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new CommonResponseDTO(ResponseConstants.StatusMessages.FAILED, "Refresh Token is missing."));
        }

        var result = await _authService.RefreshToken(refreshToken);

        var environment = HttpContext.RequestServices
            .GetRequiredService<IWebHostEnvironment>();

        Response.Cookies.Append(
            "refreshToken",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = !environment.IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/api/auth"
            });

        result.RefreshToken = string.Empty;
        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Refresh Token Received successfully.", result));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.LogoutUser(refreshToken);
        }

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            Path = "/api/auth"
        });

        return Ok(new CommonResponseDTO(ResponseConstants.StatusMessages.SUCCESS, "Logged out successfully."));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDTO request)
    {
        await _authService.ForgotPassword(request);

        // Always return 200 regardless of whether the email exists,
        // to prevent user enumeration.
        return Ok(new CommonResponseDTO(
            ResponseConstants.StatusMessages.SUCCESS,
            "If an account with that email exists, a password reset link has been sent."
        ));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDTO request)
    {
        await _authService.ResetPassword(request);

        return Ok(new CommonResponseDTO(
            ResponseConstants.StatusMessages.SUCCESS,
            "Password has been reset successfully."
        ));
    }
}
