namespace TechAssessment.Application.SponsorshipRequests.Commands.SubmitSponsorshipRequest;

public record SubmitSponsorshipRequestCommand(int Id) : IRequest<int>;
