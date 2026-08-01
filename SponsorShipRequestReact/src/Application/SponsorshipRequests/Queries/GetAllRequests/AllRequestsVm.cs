namespace TechAssessment.Application.SponsorshipRequests.Queries.GetAllRequests;

public class AllRequestsVm
{
    public IList<AllSponsorshipRequestDto> Requests { get; set; } = [];
}

public class AllSponsorshipRequestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string RequestorName { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string SponsorshipType { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
