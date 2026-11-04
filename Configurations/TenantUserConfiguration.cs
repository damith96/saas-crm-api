using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCRM.Entities;

namespace VertexCRM.Configurations;

public sealed class TenantUserConfiguration
    : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.ToTable("TenantUsers");

        builder.HasKey(m => m.Id);

        builder.HasIndex(m => m.UserId)
            .IsUnique();

        builder.HasIndex(m => new { m.TenantId, m.Role });

        builder.Property(m => m.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(m => m.IsActive)
            .HasDefaultValue(true);

        builder.Property(m => m.JoinedOn)
            .HasColumnType("timestamp with time zone");

        builder.HasOne(m => m.Tenant)
            .WithMany(t => t.Members)
            .HasForeignKey(m => m.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}