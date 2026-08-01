
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.Common.Models;
using TechAssessment.Domain.Entities;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Application.SponsorshipRequests.Commands.RejectRequest;

public class RejectRequestCommandHandler : IRequestHandler<RejectRequestCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public RejectRequestCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(RejectRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SponsorshipRequests
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (entity.Status == SponsorshipRequestStatus.Approved || entity.Status == SponsorshipRequestStatus.Rejected)
        {
            throw new InvalidOperationException("Cannot reject a request that has already been approved or rejected.");
        }

        entity.Status = SponsorshipRequestStatus.Rejected;

        if (request.ApproverRole == "Manager")
        {
            entity.ManagerApprovalRemarks = request.Remarks;
        }
        else if (request.ApproverRole == "FinanceAdmin")
        {
            entity.FinanceApprovalRemarks = request.Remarks;
        }

        var approval = new SponsorshipRequestApproval
        {
            SponsorshipRequestId = entity.Id,
            ApproverId = _user.Id ?? throw new InvalidOperationException("User ID not found."),
            ApproverRole = request.ApproverRole,
            Action = ApprovalAction.Reject,
            Comments = request.Remarks,
            ApprovedAt = DateTime.UtcNow
        };

        _context.SponsorshipRequestApprovals.Add(approval);
        await _context.SaveChangesAsync(cancellationToken);

        return approval.Id;
    }
}
