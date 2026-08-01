using Microsoft.AspNetCore.Http.HttpResults;
using TechAssessment.Application.SponsorshipRequests.Queries.GetSponsorshipTypes;

namespace TechAssessment.Web.Endpoints;

public class SponsorshipTypes : IEndpointGroup
{
    public static void Map(RouteGroupBuilder group)
    {
        group.RequireAuthorization();
        group.MapGet(GetTypes);
    }

    [EndpointSummary("Get Sponsorship Types")]
    public static async Task<Ok<SponsorshipTypesVm>> GetTypes(ICustomMediator sender)
    {
        var vm = await sender.Send(new GetSponsorshipTypesQuery());
        return TypedResults.Ok(vm);
    }
}
