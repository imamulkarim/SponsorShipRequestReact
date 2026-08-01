using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Application.Common.Models;
using TechAssessment.Domain.Entities;
using TechAssessment.Domain.Enums;

namespace TechAssessment.Application.SponsorshipRequests.Commands.ApproveRequest;

public class ApproveRequestCommandHandler : IRequestHandler<ApproveRequestCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public ApproveRequestCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
    }

    public async Task<Result<int>> Handle(ApproveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.SponsorshipRequests
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        // Validate current status based on approver role
        if (request.ApproverRole == "Manager" && entity.Status != SponsorshipRequestStatus.PendingManagerApproval)
        {
            throw new InvalidOperationException("Request is not pending manager approval.");
        }

        if (request.ApproverRole == "FinanceAdmin" && entity.Status != SponsorshipRequestStatus.PendingFinanceReview)
        {
            throw new InvalidOperationException("Request is not pending finance review.");
        }

        // Update status based on approver role
        if (request.ApproverRole == "Manager")
        {
            entity.Status = SponsorshipRequestStatus.PendingFinanceReview;
            entity.ManagerApprovalRemarks = request.Remarks;
        }
        else if (request.ApproverRole == "FinanceAdmin")
        {
            entity.Status = SponsorshipRequestStatus.Approved;
            entity.FinanceApprovalRemarks = request.Remarks;
        }

        // Record approval history
        var approval = new SponsorshipRequestApproval
        {
            SponsorshipRequestId = entity.Id,
            ApproverId = _user.Id ?? throw new InvalidOperationException("User ID not found."),
            ApproverRole = request.ApproverRole,
            Action = ApprovalAction.Approve,
            Comments = request.Remarks,
            ApprovedAt = DateTime.UtcNow
        };

        if(entity.Status == SponsorshipRequestStatus.Approved)
        {
            approval.Request = entity;
            approval.MarkFinanceApproved();
        }

        _context.SponsorshipRequestApprovals.Add(approval);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(approval.Id);
    }
}
