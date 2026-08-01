using Microsoft.AspNetCore.Http.HttpResults;
using TechAssessment.Application.SponsorshipRequests.Commands.ApproveRequest;
using TechAssessment.Application.SponsorshipRequests.Commands.RejectRequest;
using TechAssessment.Application.SponsorshipRequests.Queries.GetPendingApprovals;
using TechAssessment.Domain.Constants;

namespace TechAssessment.Web.Endpoints;

public class ManagerApprovals : IEndpointGroup
{
    public static void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(policy => policy.RequireRole(Roles.Manager));

        group.MapGet(GetPendingApprovals);
        group.MapPost(ApproveRequest, "{id}/approve");
        group.MapPost(RejectRequest, "{id}/reject");
    }

    [EndpointSummary("Get Pending Manager Approvals")]
    public static async Task<Ok<PendingApprovalsVm>> GetPendingApprovals(ICustomMediator sender)
    {
        var vm = await sender.Send(new GetPendingApprovalsQuery("Manager"));
        return TypedResults.Ok(vm);
    }

    [EndpointSummary("Approve Request")]
    public static async Task<NoContent> ApproveRequest(ICustomMediator sender, int id, ApproveRequestCommand command)
    {
        var approveCommand = command with { Id = id, ApproverRole = "Manager" };
        await sender.Send(approveCommand);
        return TypedResults.NoContent();
    }

    [EndpointSummary("Reject Request")]
    public static async Task<NoContent> RejectRequest(ICustomMediator sender, int id, RejectRequestCommand command)
    {
        var rejectCommand = command with { Id = id, ApproverRole = "Manager" };
        await sender.Send(rejectCommand);
        return TypedResults.NoContent();
    }
}
