using System.ComponentModel.DataAnnotations;

namespace VertexCRM.Entities;

public enum UserStatus
{
    Pending,
    Active,
    Inactive
}

public class User : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(200)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public UserStatus UserStatus { get; set; } = UserStatus.Pending;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;
}
