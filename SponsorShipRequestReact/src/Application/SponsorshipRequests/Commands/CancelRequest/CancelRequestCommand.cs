namespace TechAssessment.Application.SponsorshipRequests.Commands.CancelRequest;

public record CancelRequestCommand(int Id, string Reason) : IRequest<int>;
