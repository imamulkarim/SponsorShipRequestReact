using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechAssessment.Infrastructure.Identity;

namespace TechAssessment.Web.Endpoints;

public class Users : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapIdentityApi<ApplicationUser>();

        groupBuilder.MapGet(GetCurrentUser, "current").RequireAuthorization();
        groupBuilder.MapPost(Logout, "logout").RequireAuthorization();
    }

    [EndpointSummary("Get current user")]
    public static Ok<CurrentUserResponse> GetCurrentUser(ClaimsPrincipal user)
    {
        var response = new CurrentUserResponse
        {
            UserId = user.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = user.FindFirstValue(ClaimTypes.Email) ?? user.Identity?.Name,
            Roles = user.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList()
        };

        return TypedResults.Ok(response);
    }

    [EndpointSummary("Log out")]
    [EndpointDescription("Logs out the current user by clearing the authentication cookie.")]
    public static async Task<Results<Ok, UnauthorizedHttpResult>> Logout(SignInManager<ApplicationUser> signInManager, [FromBody] object empty)
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return TypedResults.Ok();
        }

        return TypedResults.Unauthorized();
    }
}

public class CurrentUserResponse
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public IList<string> Roles { get; set; } = [];
}
