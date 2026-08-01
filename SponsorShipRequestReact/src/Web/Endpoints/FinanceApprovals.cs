using Microsoft.AspNetCore.Http.HttpResults;
using TechAssessment.Application.SponsorshipRequests.Commands.ApproveRequest;
using TechAssessment.Application.SponsorshipRequests.Commands.RejectRequest;
using TechAssessment.Application.SponsorshipRequests.Queries.GetPendingApprovals;
using TechAssessment.Domain.Constants;

namespace TechAssessment.Web.Endpoints;

public class FinanceApprovals : IEndpointGroup
{
    public static void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(policy => policy.RequireRole(Roles.FinanceAdmin));

        group.MapGet(GetPendingApprovalsFinance);
        group.MapPost(ApproveRequestFinance, "{id}/approve");
        group.MapPost(RejectRequestFinance, "{id}/reject");
    }

    [EndpointSummary("Get Pending Finance Approvals")]
    public static async Task<Ok<PendingApprovalsVm>> GetPendingApprovalsFinance(ICustomMediator sender)
    {
        var vm = await sender.Send(new GetPendingApprovalsQuery("Finance"));
        return TypedResults.Ok(vm);
    }

    [EndpointSummary("Approve Request")]
    public static async Task<NoContent> ApproveRequestFinance(ICustomMediator sender, int id, ApproveRequestCommand command)
    {
        var approveCommand = command with { Id = id, ApproverRole = "FinanceAdmin" };
        await sender.Send(approveCommand);
        return TypedResults.NoContent();
    }

    [EndpointSummary("Reject Request")]
    public static async Task<NoContent> RejectRequestFinance(ICustomMediator sender, int id, RejectRequestCommand command)
    {
        var rejectCommand = command with { Id = id, ApproverRole = "FinanceAdmin" };
        await sender.Send(rejectCommand);
        return TypedResults.NoContent();
    }
}
