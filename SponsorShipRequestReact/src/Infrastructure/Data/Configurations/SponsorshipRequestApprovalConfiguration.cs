using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechAssessment.Domain.Entities;

namespace TechAssessment.Infrastructure.Data.Configurations;

public class SponsorshipRequestApprovalConfiguration : IEntityTypeConfiguration<SponsorshipRequestApproval>
{
    public void Configure(EntityTypeBuilder<SponsorshipRequestApproval> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ApproverId)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.ApproverRole)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasConversion<string>();

        builder.HasIndex(x => x.SponsorshipRequestId);
        builder.HasIndex(x => x.ApproverId);
    }
}
