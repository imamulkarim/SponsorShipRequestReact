
namespace TechAssessment.Mediator;

public class CustomMediator : ICustomMediator
{

    private readonly IServiceProvider _serviceProvider;

    public CustomMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        // Dynamically construct the exact handler interface type needed: IRequestHandler<TRequest, TResponse>
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

        // Resolve the concrete handler instance from the DI container
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for request type '{requestType.Name}'");
        }

        // Locate the HandleAsync method on the constructed handler type
        var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle));

        if (method == null)
        {
            throw new InvalidOperationException($"Handle method not found on handler for '{requestType.Name}'");
        }

        // Invoke the handler and await the resulting Task
        var task = (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
        return await task;
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        //var responseType = typeof(TResponse);

        // Dynamically construct the exact handler interface type needed: IRequestHandler<TRequest, TResponse>
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType);

        // Resolve the concrete handler instance from the DI container
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for request type '{requestType.Name}'");
        }

        // Locate the HandleAsync method on the constructed handler type
        //var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest>.Handle));

        //if (method == null)
        //{
        //    throw new InvalidOperationException($"Handle method not found on handler for '{requestType.Name}'");
        //}

        //// Invoke the handler and await the resulting Task
        //var task = (Task)method.Invoke(handler, new object[] { request, cancellationToken })!;
        //await task;
    }


    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
