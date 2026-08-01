using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TechAssessment.Application.WeatherForecasts.Queries.GetWeatherForecasts;

namespace TechAssessment.Web.Endpoints;

[Route("api/[controller]")]
[ApiController]
public class WeatherForecasts : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetWeatherForecasts);
    }

    [EndpointSummary("Get Weather Forecasts")]
    [EndpointDescription("Retrieves a list of weather forecasts for the next few days.")]
    public static async Task<Ok<IEnumerable<WeatherForecast>>> GetWeatherForecasts(ICustomMediator sender)
    {
        var forecasts = await sender.Send(new GetWeatherForecastsQuery());

        return TypedResults.Ok(forecasts);
    }
}
