using Microsoft.AspNetCore.Http.HttpResults;
using TechAssessment.Application.SponsorshipRequests.Queries.GetAllRequests;
using TechAssessment.Application.SponsorshipRequests.Queries.GetRequestDetail;
using TechAssessment.Domain.Constants;

namespace TechAssessment.Web.Endpoints;

public class AdminRequests : IEndpointGroup
{
    public static void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization(policy => policy.RequireRole(Roles.SystemAdmin));

        group.MapGet(GetAllRequests);
        group.MapGet(GetRequestDetail, "{id}");
    }

    [EndpointSummary("Get All Requests")]
    public static async Task<Ok<AllRequestsVm>> GetAllRequests(ICustomMediator sender)
    {
        var vm = await sender.Send(new GetAllRequestsQuery());
        return TypedResults.Ok(vm);
    }

    [EndpointSummary("Get Request Detail")]
    public static async Task<Ok<RequestDetailVm>> GetRequestDetail(ICustomMediator sender, int id)
    {
        var vm = await sender.Send(new GetRequestDetailQuery(id));
        return TypedResults.Ok(vm);
    }
}
