using Microsoft.Extensions.Logging;

namespace TechAssessment.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly IRequestHandler<TRequest, TResponse> _inner;

 
    public UnhandledExceptionBehaviour(ILogger<TRequest> logger, IRequestHandler<TRequest, TResponse> inner)
    {
        _logger = logger;
        _inner = inner;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return await _inner.Handle(request, cancellationToken);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, "TechAssessment Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

            throw;
        }
    }
}
