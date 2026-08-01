using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechAssessment.Domain.Entities;

namespace TechAssessment.Infrastructure.Data.Configurations;

public class SponsorshipRequestConfiguration : IEntityTypeConfiguration<SponsorshipRequest>
{
    public void Configure(EntityTypeBuilder<SponsorshipRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.RequestorId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.RequestorName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Department)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.EventName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Purpose)
            .IsRequired();

        builder.Property(x => x.RequestedAmount)
            .HasPrecision(10, 2);

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.HasMany(x => x.Approvals)
            .WithOne(x => x.Request)
            .HasForeignKey(x => x.SponsorshipRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.RequestorId);
        builder.HasIndex(x => x.Created);
    }
}
