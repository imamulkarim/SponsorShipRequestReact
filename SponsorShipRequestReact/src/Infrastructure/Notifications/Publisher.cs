using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Domain.Common;

namespace TechAssessment.Infrastructure.Notifications;


public class Publisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public Publisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        if (notification == null) throw new ArgumentNullException(nameof(notification));

        var requestType = notification.GetType();
        //var responseType = typeof(TResponse);

        // Dynamically construct the exact handler interface type needed: IRequestHandler<TRequest, TResponse>
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(requestType);

        // Resolve the concrete handler instance from the DI container
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for request type '{requestType.Name}'");
        }

        // Locate the HandleAsync method on the constructed handler type
        var method = handlerType.GetMethod(nameof(INotificationHandler<INotification>.HandleAsync));

        if (method == null)
        {
            throw new InvalidOperationException($"Handle method not found on handler for '{requestType.Name}'");
        }

        // Invoke the handler and await the resulting Task
        var task = (Task)method.Invoke(handler, new object[] { notification, cancellationToken })!;
        return Task.FromResult(task);
    }
}
