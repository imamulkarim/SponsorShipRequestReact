namespace TechAssessment.Application.SponsorshipRequests.Queries.GetPendingApprovals;

public class PendingApprovalDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string RequestorName { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
}
