using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechAssessment.Domain.Entities;

namespace TechAssessment.Infrastructure.Data.Configurations;

public class SponsorshipTypeConfiguration : IEntityTypeConfiguration<SponsorshipType>
{
    public void Configure(EntityTypeBuilder<SponsorshipType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        // Seed default sponsorship types
        builder.HasData(
            new SponsorshipType { Id = 1, Name = "Conference", Description = "Industry conference participation", IsActive = true },
            new SponsorshipType { Id = 2, Name = "Training", Description = "Professional training and certification", IsActive = true },
            new SponsorshipType { Id = 3, Name = "Community Event", Description = "Community or charitable event", IsActive = true },
            new SponsorshipType { Id = 4, Name = "Research", Description = "Research initiative sponsorship", IsActive = true }
        );
    }
}
