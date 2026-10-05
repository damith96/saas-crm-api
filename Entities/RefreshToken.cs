using System.ComponentModel.DataAnnotations;

namespace VertexCRM.Entities;

public class RefreshToken : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(512)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public virtual User User { get; set; } = null!;
}
