using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCRM.Entities;

namespace VertexCRM.Configurations;

public sealed class CompanyConfiguration
    : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.LegalName)
            .HasMaxLength(250);

        builder.Property(c => c.Website)
            .HasMaxLength(500);

        builder.Property(c => c.Industry)
            .HasMaxLength(100);

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(c => c.Email)
            .HasMaxLength(254);

        builder.Property(c => c.AddressLine1)
            .HasMaxLength(250);

        builder.Property(c => c.AddressLine2)
            .HasMaxLength(250);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.StateOrProvince)
            .HasMaxLength(100);

        builder.Property(c => c.PostalCode)
            .HasMaxLength(20);

        builder.Property(c => c.CountryCode)
            .HasMaxLength(2);

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedOn)
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.UpdatedOn)
            .HasColumnType("timestamp with time zone");

        builder.Property(c => c.DeletedOn)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(c => new
        {
            c.TenantId,
            c.IsDeleted,
            c.Name
        });

        builder.HasIndex(c => new
        {
            c.TenantId,
            c.CreatedOn
        });

        builder.HasOne(c => c.Tenant)
            .WithMany(t => t.Companies)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.DeletedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}