
using System.ComponentModel.DataAnnotations;

public sealed class UpdateCompanyRequestDTO
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
