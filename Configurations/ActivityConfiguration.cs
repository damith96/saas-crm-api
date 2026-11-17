using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCRM.Entities;

public sealed class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(a => a.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(3000);

        builder.Property(a => a.IsDeleted)
            .HasDefaultValue(false);

        //Company relationship
        builder.HasOne(a => a.Company)
            .WithMany(c => c.Activities)
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        //Contact relationship (optional)
        builder.HasOne(a => a.Contact)
            .WithMany(c => c.Activities)
            .HasForeignKey(a => a.CompanyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        //Deal relationship (optional)
        builder.HasOne(a => a.Deal)
            .WithMany(c => c.Activities)
            .HasForeignKey(a => a.DealId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        //Indexes
        builder.HasIndex(a => a.CompanyId);
        builder.HasIndex(a => a.ContactId);
        builder.HasIndex(a => a.DealId);
        builder.HasIndex(a => a.ActivityDate);
        builder.HasIndex(a => new { a.CompanyId, a.ActivityDate });


    }
}