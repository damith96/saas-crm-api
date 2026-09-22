using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VertexCRM.Data;
using VertexCRM.DTOs.Auth;
using VertexCRM.Entities;
using VertexCRM.Interfaces;

namespace VertexCRM.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
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
}
