using Microsoft.Extensions.Logging;
using TechAssessment.Application.Common.Interfaces;
using TechAssessment.Domain.Events;

namespace TechAssessment.Application.SponsorshipRequests.Events;

public class SponsorShipFinanceApprovalEventHandler : INotificationHandler<SponsorShipApprovedEvent>
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IServiceBusPublisher _serviceBusPublisher;
    private readonly ISponsorshipFinanceApiClientService _sponsorshipFinanceApiClientService;
    public SponsorShipFinanceApprovalEventHandler(ILogger<SponsorShipApprovedEvent> logger, IUser user, IServiceBusPublisher serviceBusPublisher, ISponsorshipFinanceApiClientService sponsorshipFinanceApiClientService)
    {
        _logger = logger;
        _user = user;
        _serviceBusPublisher = serviceBusPublisher;
        _sponsorshipFinanceApiClientService = sponsorshipFinanceApiClientService;
    }

    public Task HandleAsync(SponsorShipApprovedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handling {notification}");

        //Read the readme file for more information on how to implement the event handler logic.

        var sponsorRequestDto = new SponsorShipRequestDto
        {
            Id = notification.FinanceApprovedEvent.Id,
            Title = notification.FinanceApprovedEvent.Title,
            RequestorName = notification.FinanceApprovedEvent.RequestorName,
            Department = notification.FinanceApprovedEvent.Department,
            SponsorshipType = notification.FinanceApprovedEvent.SponsorshipTypeId.ToString(),
            EventName = notification.FinanceApprovedEvent.EventName,
            EventDate = notification.FinanceApprovedEvent.EventDate,
            RequestedAmount = notification.FinanceApprovedEvent.RequestedAmount,
            Purpose = notification.FinanceApprovedEvent.Purpose,
            Status = notification.FinanceApprovedEvent.Status.ToString(),
            ManagerApprovalRemarks = notification.FinanceApprovedEvent.ManagerApprovalRemarks,
            FinanceApprovalRemarks = notification.FinanceApprovedEvent.FinanceApprovalRemarks,
            CreatedAt = notification.FinanceApprovedEvent.Created.UtcDateTime,
            LastModifiedAt = notification.FinanceApprovedEvent.LastModified.UtcDateTime
        };

        _serviceBusPublisher.PublishAsync(sponsorRequestDto.Id.ToString(), sponsorRequestDto, "sponsor-finance-approved-queue", cancellationToken);
        _logger.LogInformation("TechAssessment Request: Finance Approved Service is handled : {@Id}",
            notification.FinanceApprovedEvent.Id.ToString());

        

         _sponsorshipFinanceApiClientService.PostSponsorshipRequestAmount(notification, cancellationToken);

        return Task.CompletedTask;
    }
}
