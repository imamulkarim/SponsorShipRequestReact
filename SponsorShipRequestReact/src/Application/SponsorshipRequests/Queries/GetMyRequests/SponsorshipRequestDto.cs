namespace TechAssessment.Application.SponsorshipRequests.Queries.GetMyRequests;

public class SponsorshipRequestDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string SponsorshipType { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
