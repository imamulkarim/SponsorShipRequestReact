namespace TechAssessment.Domain.Entities;

public class SponsorshipRequest : BaseAuditableEntity
{
    public string RequestorId { get; set; } = null!;
    public string RequestorName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Department { get; set; } = null!;
    public int SponsorshipTypeId { get; set; }
    public string EventName { get; set; } = null!;
    public DateTime EventDate { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Purpose { get; set; } = null!;
    public string? BusinessBenefit { get; set; }
    public string? SupportingDocumentUrl { get; set; }
    public SponsorshipRequestStatus Status { get; set; }
    public string? ManagerApprovalRemarks { get; set; }
    public string? FinanceApprovalRemarks { get; set; }
    public string? CancelledReason { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Navigation properties
    public ICollection<SponsorshipRequestApproval> Approvals { get; set; } = [];
}
