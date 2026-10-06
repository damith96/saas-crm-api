using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VertexCRM.Data;
using VertexCRM.DTOs.Auth;
using VertexCRM.Entities;
using VertexCRM.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;

namespace VertexCRM.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
        _configuration = configuration;
    }

    public async Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO request)
    {
        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserStatus = UserStatus.Pending,
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new AuthResponseDTO
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDTO> LoginUser(LoginRequestDTO request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Your account is inactive.");
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var accessToken = GenerateJwtToken(user);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow
        };

        user.LastLoginAt = DateTime.UtcNow;
        _context.RefreshTokens.Add(refreshToken);

        await _context.SaveChangesAsync();

        return new AuthResponseDTO
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthResponseDTO> RefreshToken(RefreshTokenRequestDTO request)
{
    var refreshToken = await _context.RefreshTokens
        .Include(x => x.User)
        .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

    if (refreshToken == null)
    {
        throw new UnauthorizedAccessException("Invalid refresh token.");
    }

    if (refreshToken.IsRevoked)
    {
        throw new UnauthorizedAccessException("Refresh token has been revoked.");
    }

    if (refreshToken.ExpiresAt <= DateTime.UtcNow)
    {
        throw new UnauthorizedAccessException("Refresh token has expired.");
    }

    var user = refreshToken.User;

    if (!user.IsActive)
    {
        throw new UnauthorizedAccessException("User account is inactive.");
    }

    // Revoke the old refresh token
    refreshToken.RevokedAt = DateTime.UtcNow;

    // Create a new refresh token
    var newRefreshToken = new RefreshToken
    {
        UserId = user.Id,
        Token = GenerateRefreshToken(),
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        CreatedOn = DateTime.UtcNow
    };

    _context.RefreshTokens.Add(newRefreshToken);

    // Create a new access token
    var accessToken = GenerateAccessToken(user);

    await _context.SaveChangesAsync();

    return new AuthResponseDTO
    {
        UserId = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        AccessToken = accessToken,
        RefreshToken = newRefreshToken.Token
    };
}
    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is not configured.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)
        );

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}
