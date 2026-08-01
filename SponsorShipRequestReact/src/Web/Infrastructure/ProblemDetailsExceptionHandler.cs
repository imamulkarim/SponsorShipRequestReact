using TechAssessment.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace TechAssessment.Web.Infrastructure;

/// <summary>
/// Converts well-known application exceptions into RFC 9110-compliant <see cref="ProblemDetails"/> responses,
/// mapping <see cref="ValidationException"/> → 400, <see cref="NotFoundException"/> → 404,
/// <see cref="UnauthorizedAccessException"/> → 401, and <see cref="ForbiddenAccessException"/> → 403.
/// Unrecognised exceptions are not handled and fall through to the default middleware.
/// </summary>
public class ProblemDetailsExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        //Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, (ProblemDetails)new ValidationProblemDetails(ve.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = ve.GetType().Name, //"https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "One or more validation errors occurred.",
                Detail = ve.Message,
                //As this same code require for all block, we can move that code in service regisistration part in program.cs file.
                //Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                //Extensions =
                //{
                //    ["requestId"] = httpContext.TraceIdentifier,
                //    ["traceId"] = activity?.Id
                //}
            }),
            NotFoundException ne => (StatusCodes.Status404NotFound, new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Type = ne.GetType().Name, //"https://tools.ietf.org/html/rfc9110#section-15.5.5",
                Title = "The specified resource was not found.",
                Detail = ne.Message
            }),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            }),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
            }),
            _ => (-1, null)
        };

        if (problemDetails is null) return false;

        httpContext.Response.StatusCode = statusCode;
        //await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        //return true;

        //Using the ProblemDetailsService to write the response, so that it can be customized in one place.
        //https://www.youtube.com/watch?v=eN4GX5WW87s
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext { 
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails,
        });
    }
}
