namespace TechAssessment.Application.SponsorshipRequests.Queries.GetPendingApprovals;

public record GetPendingApprovalsQuery(string ApprovalStage) : IRequest<PendingApprovalsVm>;
