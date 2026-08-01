namespace TechAssessment.Application.SponsorshipRequests.Commands.UpdateSponsorshipRequest;

public record UpdateSponsorshipRequestCommand : IRequest<int>
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Department { get; init; }
    public required int SponsorshipTypeId { get; init; }
    public required string EventName { get; init; }
    public required DateTime EventDate { get; init; }
    public required decimal RequestedAmount { get; init; }
    public required string Purpose { get; init; }
    public string? BusinessBenefit { get; init; }
    public string? SupportingDocumentUrl { get; init; }
}
