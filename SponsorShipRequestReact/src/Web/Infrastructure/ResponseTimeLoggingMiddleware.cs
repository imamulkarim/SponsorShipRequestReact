using System.Diagnostics;

namespace TechAssessment.Web.Infrastructure;

public class ResponseTimeLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseTimeLoggingMiddleware> _logger;

    public ResponseTimeLoggingMiddleware(RequestDelegate next, ILogger<ResponseTimeLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var responseTime = stopwatch.ElapsedMilliseconds;
            _logger.LogInformation("Request {Method} {Path} responded in {ResponseTime} ms", context.Request.Method, context.Request.Path, responseTime);
        }
        
    }
}
