using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.RateLimiting;
using TechAssessment.Application.SponsorshipRequests.Commands.CancelRequest;
using TechAssessment.Application.SponsorshipRequests.Commands.CreateSponsorshipRequest;
using TechAssessment.Application.SponsorshipRequests.Commands.SubmitSponsorshipRequest;
using TechAssessment.Application.SponsorshipRequests.Commands.UpdateSponsorshipRequest;
using TechAssessment.Application.SponsorshipRequests.Queries.GetMyRequests;
using TechAssessment.Application.SponsorshipRequests.Queries.GetRequestDetail;

namespace TechAssessment.Web.Endpoints;

public class SponsorshipRequests : IEndpointGroup
{
    public static void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization();
        group.RequireRateLimiting("fixed");

        group.MapPost(CreateRequest);
        group.MapGet(GetMyRequests);
        group.MapGet(GetMyRequestsDetail, "{id}");
        group.MapPut(UpdateRequest, "{id}");
        group.MapPost(SubmitRequest, "{id}/submit");
        group.MapPost(CancelRequest, "{id}/cancel");
    }

    [EndpointSummary("Create Sponsorship Request")]
    public static async Task<Created<int>> CreateRequest(ICustomMediator sender, CreateSponsorshipRequestCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/sponsorship-requests/{id}", id);
    }

    [EndpointSummary("Get My Sponsorship Requests")]
    [EnableRateLimiting("per-user")]
    public static async Task<Ok<MyRequestsVm>> GetMyRequests(ICustomMediator sender)
    {
        var vm = await sender.Send(new GetMyRequestsQuery());
        return TypedResults.Ok(vm);
    }

    [EndpointSummary("Get Request Detail")]
    public static async Task<Ok<RequestDetailVm>> GetMyRequestsDetail(ICustomMediator sender, int id)
    {
        var vm = await sender.Send(new GetRequestDetailQuery(id));
        return TypedResults.Ok(vm);
    }

    [EndpointSummary("Update Sponsorship Request")]
    public static async Task<NoContent> UpdateRequest(ICustomMediator sender, int id, UpdateSponsorshipRequestCommand command)
    {
        if (id != command.Id) return TypedResults.NoContent();
        await sender.Send(command);
        return TypedResults.NoContent();
    }

    [EndpointSummary("Submit Sponsorship Request")]
    public static async Task<NoContent> SubmitRequest(ICustomMediator sender, int id)
    {
        await sender.Send(new SubmitSponsorshipRequestCommand(id));
        return TypedResults.NoContent();
    }

    [EndpointSummary("Cancel Sponsorship Request")]
    public static async Task<NoContent> CancelRequest(ICustomMediator sender, int id, CancelRequestCommand command)
    {
        if (id != command.Id) return TypedResults.NoContent();
        await sender.Send(command);
        return TypedResults.NoContent();
    }
}
