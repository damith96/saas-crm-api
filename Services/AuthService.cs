using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VertexCRM.Data;
using VertexCRM.DTOs.Auth;
using VertexCRM.Entities;
using VertexCRM.Exceptions;
using VertexCRM.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VertexCRM.Constants;

namespace VertexCRM.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(
        ApplicationDbContext context,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<AuthResponseDTO> RegisterUser(RegisterRequestDTO request)
    {
        // AppException(Conflict) — email already taken
        var emailExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email);

        if (emailExists)
            throw new AppException(HttpStatusCode.Conflict, "Email is already registered.");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = UserStatus.Pending,
            RoleId = RoleConstants.MemberId,   // default role — seeded in RoleConfiguration
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
            .Include(u => u.Role)
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        if (user == null)
            throw new AppException(HttpStatusCode.Unauthorized, "Invalid email or password.");

        if (!user.IsActive)
            throw new AppException(HttpStatusCode.Conflict, "Your account is inactive.");

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordResult == PasswordVerificationResult.Failed)
            throw new AppException(HttpStatusCode.Unauthorized, "Invalid email or password.");

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

    public async Task<AuthResponseDTO> RefreshToken(string refreshTokenValue)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshTokenValue);

        if (refreshToken == null)
            throw new AppException(HttpStatusCode.Unauthorized, "Invalid refresh token.");

        if (refreshToken.IsRevoked)
            throw new AppException(HttpStatusCode.Unauthorized, "Refresh token has been revoked.");

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            throw new AppException(HttpStatusCode.Unauthorized, "Refresh token has expired.");

        var user = refreshToken.User;

        if (!user.IsActive)
            throw new AppException(HttpStatusCode.Conflict, "User account is inactive.");

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
        var accessToken = GenerateJwtToken(user);

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

    public async Task LogoutUser(string refreshTokenValue)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshTokenValue);

        // Idempotent logout: unknown or already-revoked tokens need no action.
        if (refreshToken == null || refreshToken.IsRevoked)
        {
            return;
        }

        refreshToken.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task ForgotPassword(ForgotPasswordRequestDTO request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);

        // Always return success to avoid leaking whether the email exists.
        if (user == null || !user.IsActive)
            return;

        // Invalidate any existing unused tokens for this user.
        var existingTokens = await _context.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var t in existingTokens)
            t.IsUsed = true;

        // Generate a new secure token.
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = rawToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            CreatedOn = DateTime.UtcNow
        };

        _context.PasswordResetTokens.Add(resetToken);
        await _context.SaveChangesAsync();

        await _emailService.SendPasswordResetEmailAsync(user.Email, rawToken);
    }

    public async Task ResetPassword(ResetPasswordRequestDTO request)
    {
        var resetToken = await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token);

        if (resetToken == null || resetToken.IsUsed)
            throw new AppException(HttpStatusCode.BadRequest, "Invalid or expired password reset token.");

        if (resetToken.ExpiresAt <= DateTime.UtcNow)
            throw new AppException(HttpStatusCode.BadRequest, "Invalid or expired password reset token.");

        var user = resetToken.User;

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        // Mark the token as used and revoke all refresh tokens for this user
        // so existing sessions are terminated after a password change.
        resetToken.IsUsed = true;

        var activeRefreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == user.Id && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var rt in activeRefreshTokens)
            rt.RevokedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    private string GenerateJwtToken(User user)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is not configured.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is not configured.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is not configured.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),

            new(ClaimTypes.Role, user.Role.Name),
            new("role_scope", user.Role.Scope.ToString())
        };

        // Only add tenant_id when the user belongs to a tenant.
        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim(
                "tenant_id",
                user.TenantId.Value.ToString()
            ));
        }
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
