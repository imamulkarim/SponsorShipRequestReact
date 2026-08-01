namespace TechAssessment.Application.SponsorshipRequests.Queries.GetSponsorshipTypes;

public class SponsorshipTypesVm
{
    public IList<SponsorshipTypeDto> SponsorshipTypes { get; set; } = [];
}

public class SponsorshipTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
}
