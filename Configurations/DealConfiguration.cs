using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCRM.Constants;
using VertexCRM.Entities;

namespace VertexCRM.Configurations;

public sealed class DealConfiguration
    : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.ToTable("Deals");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .HasMaxLength(2000);

        builder.Property(d => d.Amount)
            .HasPrecision(18, 2);

        builder.Property(d => d.CurrencyCode)
            .HasMaxLength(3);

        builder.Property(d => d.Stage)
            .HasConversion<int>()
            .HasDefaultValue(DealStage.New);

        builder.Property(d => d.IsDeleted)
            .HasDefaultValue(false);

        // Company relationship
        builder.HasOne(d => d.Company)
            .WithMany(c => c.Deals)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional Contact relationship
        builder.HasOne(d => d.Contact)
            .WithMany(c => c.Deals)
            .HasForeignKey(d => d.ContactId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.CompanyId);
        builder.HasIndex(d => d.ContactId);
    }
}