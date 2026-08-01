

using TechAssessment.Domain.Common;

namespace TechAssessment.Application.Common.Interfaces;

public interface IPublisher
{
    //
    // Summary:
    //     Asynchronously send a notification to multiple handlers
    //
    // Parameters:
    //   notification:
    //     Notification object
    //
    //   cancellationToken:
    //     Optional cancellation token
    //
    // Returns:
    //     A task that represents the publish operation.
    Task Publish(object notification, CancellationToken cancellationToken = default(CancellationToken));

    //
    // Summary:
    //     Asynchronously send a notification to multiple handlers
    //
    // Parameters:
    //   notification:
    //     Notification object
    //
    //   cancellationToken:
    //     Optional cancellation token
    //
    // Returns:
    //     A task that represents the publish operation.
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default(CancellationToken)) where TNotification : INotification;

}
