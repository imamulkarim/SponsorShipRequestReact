using TechAssessment.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace TechAssessment.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private readonly IRequestHandler<TRequest, TResponse> _inner;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService, IRequestHandler<TRequest, TResponse> inner)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
        _inner = inner;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user?.Id ?? string.Empty;
        string? userName = string.Empty;

        _logger.LogInformation($"Handling {requestName}");

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("TechAssessment Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        try
        {
            var response = await _inner.Handle(request, cancellationToken);

            return response;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Failed handling {requestName}");
            throw;
        }
        finally
        {
            _logger.LogInformation($"Completed handling {requestName}");
        }

    }
}


/*
 * Backup copy
 * public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("TechAssessment Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);
    }
}

 * */
