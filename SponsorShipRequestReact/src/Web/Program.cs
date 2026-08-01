using System.Reflection;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Polly;
using Scalar.AspNetCore;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Infrastructure.Data;
using TechAssessment.Infrastructure.Services.SponsorshipFinance;
using TechAssessment.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Stop using .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
builder.AddServiceDefaults();

//builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();

builder.AddWebServices();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new()
//    {
//        Title = "Clean Architecture API",
//        Version = "v1",
//        Description = "Demo Clean Architecture API"
//    });
//});

//builder.Services.AddSingleton<ResponseTimeLoggingMiddleware>();

builder.Services.AddRateLimiter(options =>
{

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        //context.HttpContext.Response.Headers.RetryAfter = "1";
        //await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);

        if(context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();

            ProblemDetailsFactory problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                statusCode: StatusCodes.Status429TooManyRequests,
                title: "Too many requests",
                detail: $"You have exceeded the rate limit. Please try again after {retryAfter}."
            );
            await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }
    };

    //Check SponsorshipRequests endpoint for usage of the rate limiters below.
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    options.AddPolicy("per-user", context =>
    {
        var userId = context.User?.Identity?.Name ?? "anonymous";

        if(userId == "anonymous")
        {
            return RateLimitPartition.GetFixedWindowLimiter("anonymous", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            });
        }

        return RateLimitPartition.GetTokenBucketLimiter(userId, _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 5,
            TokensPerPeriod =2,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1)
        });
    });
});

//Installed the NuGet package: Microsoft.AspNetCore.HeaderPropagation
builder.Services.AddHeaderPropagation(options =>
{
    // Tell it to capture the 'Cookie' header from incoming requests
    options.Headers.Add("Cookie");
});

builder.Services.AddHttpClient<ISponsorshipFinanceApiClientService, SponsorshipFinanceApiClientService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["SponsorshipFinanceApi:BaseUrl"] ?? throw new InvalidOperationException("SponsorshipFinanceApi:BaseUrl is not configured."));
    client.DefaultRequestHeaders.Add("Accept", "application/json");

    // The total execution time ceiling for the entire operation (including retries)
    client.Timeout = TimeSpan.FromSeconds(30); // Set a timeout for the HTTP requests

    // As this example using Authentication, by AddIdentityCookies so we need to capture the headers from the incoming request and forward them to the outgoing request. This is important for scenarios where the API requires authentication.

})
.AddHeaderPropagation() // Add this line to enable header propagation
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    // RULE 1: Fast Connect Timeout. Don't waste time on dead servers.
    ConnectTimeout = TimeSpan.FromSeconds(2),
})
//.AddStandardResilienceHandler(option =>
//{
//    option.Retry.MaxRetryAttempts = 5;
//    option.Retry.BackoffType = DelayBackoffType.Exponential;

//    // Configure circuit breaker to open if 50% of requests fail within 10 seconds
//    option.CircuitBreaker.FailureRatio = 0.5;
//    option.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
//})
//This is another options
.AddResilienceHandler("SponsorshipFinanceApi", pipelineBuilder =>
{
    // RULE 2: Read/Response Timeout per individual attempt
    pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(8));

    // RULE 3: Retry with Exponential Backoff and Jitter
    pipelineBuilder.AddRetry(new Polly.Retry.RetryStrategyOptions<HttpResponseMessage>
    {
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>() // Handles network/connection drops
            .HandleResult(res => (int)res.StatusCode >= 500) // Handles server errors (5xx)
        ,
        MaxRetryAttempts = 3,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        Delay = TimeSpan.FromSeconds(1)
    });

    // RULE 4: Circuit Breaker to stop hitting a failing API
    pipelineBuilder.AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions<HttpResponseMessage>
    {
        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
            .Handle<HttpRequestException>() // Handles network/connection drops
            .HandleResult(res => (int)res.StatusCode >= 500) // Handles server errors (5xx)
        ,
        FailureRatio = 0.5, // 50% failure rate
        SamplingDuration = TimeSpan.FromSeconds(10), // Over a 10-second window
        MinimumThroughput = 8, // Minimum of 5 requests in the sampling duration
        BreakDuration = TimeSpan.FromSeconds(30) // Break for 30 seconds
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();

    //app.UseSwagger();
    //app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors(static builder => 
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());

app.UseFileServer();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseExceptionHandler(); //options => { }
app.UseStatusCodePages();

app.UseMiddleware<ResponseTimeLoggingMiddleware>();
app.UseRateLimiter();

app.UseHeaderPropagation();

// Stop using .NET Aspire services:
app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);

app.MapFallbackToFile("index.html");


app.Run();
