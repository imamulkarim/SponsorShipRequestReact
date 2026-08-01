using System.Diagnostics;
using TechAssessment.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace TechAssessment.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private readonly IRequestHandler<TRequest, TResponse> _inner;
    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        IUser user,
        IIdentityService identityService,
        IRequestHandler<TRequest, TResponse> inner)
    {
        _timer = new Stopwatch();

        _logger = logger;
        _user = user;
        _identityService = identityService;
        _inner = inner;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await _inner.Handle(request, cancellationToken); //next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user?.Id ?? string.Empty;
            var userName = string.Empty;

            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.LogWarning("TechAssessment Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                requestName, elapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}
