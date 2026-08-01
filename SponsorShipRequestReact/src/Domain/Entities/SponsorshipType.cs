
namespace TechAssessment.Domain.Entities;

public class SponsorshipType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
}
