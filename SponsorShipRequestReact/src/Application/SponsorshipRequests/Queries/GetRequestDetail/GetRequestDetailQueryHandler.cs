using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using TechAssessment.Application.Common.Interfaces;

namespace TechAssessment.Application.SponsorshipRequests.Queries.GetRequestDetail;

public class GetRequestDetailQueryHandler : IRequestHandler<GetRequestDetailQuery, RequestDetailVm>
{
    private readonly IApplicationDbContext _context;

    public GetRequestDetailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RequestDetailVm> Handle(GetRequestDetailQuery request, CancellationToken cancellationToken)
    {
        var sponsorshipRequest = await _context.SponsorshipRequests
            .Include(x => x.Approvals)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, sponsorshipRequest);

        return new RequestDetailVm
        {
            Id = sponsorshipRequest.Id,
            Title = sponsorshipRequest.Title,
            RequestorName = sponsorshipRequest.RequestorName,
            Department = sponsorshipRequest.Department,
            SponsorshipType = sponsorshipRequest.SponsorshipTypeId.ToString(),
            EventName = sponsorshipRequest.EventName,
            EventDate = sponsorshipRequest.EventDate,
            RequestedAmount = sponsorshipRequest.RequestedAmount,
            Purpose = sponsorshipRequest.Purpose,
            BusinessBenefit = sponsorshipRequest.BusinessBenefit,
            SupportingDocumentUrl = sponsorshipRequest.SupportingDocumentUrl,
            Status = sponsorshipRequest.Status.ToString(),
            ManagerApprovalRemarks = sponsorshipRequest.ManagerApprovalRemarks,
            FinanceApprovalRemarks = sponsorshipRequest.FinanceApprovalRemarks,
            CreatedAt = sponsorshipRequest.Created.DateTime,
            LastModifiedAt = sponsorshipRequest.LastModified.DateTime,
            ApprovalHistory = sponsorshipRequest.Approvals
                .OrderBy(x => x.ApprovedAt)
                .Select(x => new ApprovalHistoryDto
                {
                    Id = x.Id,
                    ApproverId = x.ApproverId,
                    ApproverRole = x.ApproverRole,
                    Action = x.Action.ToString(),
                    Comments = x.Comments,
                    ApprovedAt = x.ApprovedAt
                })
                .ToList()
        };
    }
}
