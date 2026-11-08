using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCRM.Entities;

namespace VertexCRM.Configurations;

public sealed class UserCompanyConfiguration : IEntityTypeConfiguration<UserCompany>
{
    public void Configure(EntityTypeBuilder<UserCompany> builder)
    {
        builder.ToTable("UserCompanies");

        builder.HasKey(uc => uc.Id);

        builder.Property(uc => uc.IsActive)
            .HasDefaultValue(true);

        builder.Property(uc => uc.AssignedOn)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(uc => uc.CreatedOn)
            .HasColumnType("timestamp with time zone");

        // A user can only be assigned to a given company once.
        builder.HasIndex(uc => new { uc.UserId, uc.CompanyId })
            .IsUnique();

        // Index to quickly find all users assigned to a company.
        builder.HasIndex(uc => uc.CompanyId);

        builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserCompanies)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uc => uc.Company)
            .WithMany(c => c.UserCompanies)
            .HasForeignKey(uc => uc.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // AssignedByUserId — no nav property, just a FK reference for audit.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(uc => uc.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
