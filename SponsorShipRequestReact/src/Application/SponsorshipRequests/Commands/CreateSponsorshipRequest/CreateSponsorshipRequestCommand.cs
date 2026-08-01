using System;
using System.Collections.Generic;
using System.Text;
using TechAssessment.Application.Common.Models;

namespace TechAssessment.Application.SponsorshipRequests.Commands.CreateSponsorshipRequest;

public record CreateSponsorshipRequestCommand : IRequest<int>
{
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
