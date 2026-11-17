using Microsoft.EntityFrameworkCore;
using VertexCRM.Interfaces;
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
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<CrmTask> Tasks => Set<CrmTask>();
    public DbSet<Activity> Activities => Set<Activity>();

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
                !c.Company.IsDeleted &&
                !c.IsDeleted
            );

        modelBuilder.Entity<Deal>()
            .HasQueryFilter(d =>
                _tenantContext.TenantId.HasValue &&
                d.Company.TenantId == _tenantContext.TenantId.Value &&
                !d.Company.IsDeleted &&
                !d.IsDeleted
            );

        modelBuilder.Entity<CrmTask>()
        .HasQueryFilter(t =>
            _tenantContext.TenantId.HasValue &&
            t.Company.TenantId == _tenantContext.TenantId.Value &&
            !t.Company.IsDeleted &&
            !t.IsDeleted);

        modelBuilder.Entity<Activity>()
        .HasQueryFilter(a =>
            _tenantContext.TenantId.HasValue &&
            a.Company.TenantId == _tenantContext.TenantId.Value &&
            !a.Company.IsDeleted &&
            !a.IsDeleted);
    }
}
