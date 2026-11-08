using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCRM.Constants;
using VertexCRM.Entities;

namespace VertexCRM.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(500);

        builder.Property(r => r.Scope)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(r => r.Name)
            .IsUnique();

        // Seed default roles.
        // IDs are fixed so FK references in application code are stable.
        builder.HasData(
            new Role
            {
                Id = RoleConstants.MemberId,
                Name = RoleConstants.Member,
                Description = "Default role for self-registered users. Standard CRM access within a tenant.",
                Scope = RoleScope.Tenant
            },
            new Role
            {
                Id = RoleConstants.TenantAdminId,
                Name = RoleConstants.TenantAdmin,
                Description = "Full administrative control over a tenant workspace.",
                Scope = RoleScope.Tenant
            },
            new Role
            {
                Id = RoleConstants.PlatformAdminId,
                Name = RoleConstants.PlatformAdmin,
                Description = "Global platform administrator with cross-tenant access.",
                Scope = RoleScope.Platform
            }
        );
    }
}
