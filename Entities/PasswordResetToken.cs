using System.ComponentModel.DataAnnotations;

namespace VertexCRM.Entities;

public class PasswordResetToken : BaseEntity
{
    public int UserId { get; set; }

    [Required]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; } = false;

    // Navigation property
    public virtual User User { get; set; } = null!;
}
