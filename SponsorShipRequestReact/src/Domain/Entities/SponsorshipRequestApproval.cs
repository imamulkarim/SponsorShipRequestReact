using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using TechAssessment.Domain.Events;

namespace TechAssessment.Domain.Entities;

public class SponsorshipRequestApproval : BaseEntity
{
    public int SponsorshipRequestId { get; set; }
    public string ApproverId { get; set; } = null!;
    public string ApproverRole { get; set; } = null!;
    public ApprovalAction Action { get; set; }
    public string Comments { get; set; } = null!;
    public DateTime ApprovedAt { get; set; }

    // Navigation properties
    public SponsorshipRequest Request { get; set; } = null!;
    
    [NotMapped]
    public bool IsDone { get; private set; }

    public void MarkFinanceApproved()
    {
        if (IsDone) return; // Prevent duplicate execution

        IsDone = true;

        // Register the event safely inside the tracking collection
        AddDomainEvent(new SponsorShipApprovedEvent(Request));
    }
}
