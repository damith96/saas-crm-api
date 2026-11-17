
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCRM.Entities;
using VertexCRM.Constants;

namespace VertexCRM.Configurations;

public sealed class CrmTaskConfiguration : IEntityTypeConfiguration<Entities.CrmTask>
{
    public void Configure(EntityTypeBuilder<CrmTask> builder)
    {
        builder.ToTable("CrmTasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .HasConversion<int>()
            .HasDefaultValue(CrmTaskStatus.Pending);

        builder.Property(t => t.Priority)
            .HasConversion<int>()
            .HasDefaultValue(CrmTaskPriority.Medium);

        builder.Property(t => t.IsDeleted)
            .HasDefaultValue(false);

        // Company relationship
        builder.HasOne(t => t.Company)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Contact relationship (optional)
        builder.HasOne(t => t.Contact)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.ContactId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Deal relationship (optional)
        builder.HasOne(t => t.Deal)
            .WithMany(d => d.Tasks)
            .HasForeignKey(t => t.DealId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Assigned user relationship (optional)
        builder.HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(t => t.CompanyId);
        builder.HasIndex(t => t.ContactId);
        builder.HasIndex(t => t.DealId);
        builder.HasIndex(t => t.AssignedToUserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.DueDate);
    }
}
