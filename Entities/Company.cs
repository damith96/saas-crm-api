namespace VertexCRM.Entities;

public sealed class Company : BaseEntity
{
    public int TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? LegalName { get; set; }

    public string? Website { get; set; }

    public string? Industry { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    // Address information
    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? StateOrProvince { get; set; }

    public string? PostalCode { get; set; }

    public string? CountryCode { get; set; }

    // Audit information
    public int CreatedByUserId { get; set; }

    public int? UpdatedByUserId { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
        = DateTimeOffset.UtcNow;

    // Soft deletion
    public bool IsDeleted { get; set; } = false;

    public DateTimeOffset? DeletedOn { get; set; }

    public int? DeletedByUserId { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;

    public ICollection<UserCompany> UserCompanies { get; set; }
        = new List<UserCompany>();
}