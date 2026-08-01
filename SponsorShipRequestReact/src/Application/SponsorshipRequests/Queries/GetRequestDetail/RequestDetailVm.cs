namespace TechAssessment.Application.SponsorshipRequests.Queries.GetRequestDetail;

public class RequestDetailVm
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string RequestorName { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string SponsorshipType { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = null!;
    public string? BusinessBenefit { get; set; }
    public string? SupportingDocumentUrl { get; set; }
    public string Status { get; set; } = null!;
    public string? ManagerApprovalRemarks { get; set; }
    public string? FinanceApprovalRemarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public IList<ApprovalHistoryDto> ApprovalHistory { get; set; } = [];
}

public class ApprovalHistoryDto
{
    public int Id { get; set; }
    public string ApproverId { get; set; } = null!;
    public string ApproverRole { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string Comments { get; set; } = null!;
    public DateTime ApprovedAt { get; set; }
}
