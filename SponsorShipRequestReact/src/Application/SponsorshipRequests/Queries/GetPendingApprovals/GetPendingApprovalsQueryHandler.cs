using Microsoft.EntityFrameworkCore;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Application.SponsorshipRequests.Queries.GetPendingApprovals;

public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, PendingApprovalsVm>
{
    private readonly IApplicationDbContext _context;

    public GetPendingApprovalsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PendingApprovalsVm> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var status = request.ApprovalStage == "Manager"
            ? SponsorshipRequestStatus.PendingManagerApproval
            : SponsorshipRequestStatus.PendingFinanceReview;

        var approvals = await _context.SponsorshipRequests
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.Created)
            .Select(x => new PendingApprovalDto
            {
                Id = x.Id,
                Title = x.Title,
                RequestorName = x.RequestorName,
                Department = x.Department,
                EventName = x.EventName,
                EventDate = x.EventDate,
                RequestedAmount = x.RequestedAmount,
                Purpose = x.Purpose,
                SubmittedAt = x.Created.DateTime
            })
            .ToListAsync(cancellationToken);

        return new PendingApprovalsVm { PendingApprovals = approvals };
    }
}
