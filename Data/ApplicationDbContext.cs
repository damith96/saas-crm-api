using Microsoft.EntityFrameworkCore;
using VertexCRM.Abstractions;
using VertexCRM.Entities;

namespace VertexCRM.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;


    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<UserCompany> UserCompanies => Set<UserCompany>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly
        );

        modelBuilder.Entity<Company>()
            .HasQueryFilter(c =>
                _tenantContext.TenantId.HasValue &&
                c.TenantId == _tenantContext.TenantId.Value &&
                !c.IsDeleted
            );

        modelBuilder.Entity<Contact>()
            .HasQueryFilter(c =>
                _tenantContext.TenantId.HasValue &&
                c.Company.TenantId == _tenantContext.TenantId.Value &&
                !c.Company.IsDeleted
            );
    }
}
