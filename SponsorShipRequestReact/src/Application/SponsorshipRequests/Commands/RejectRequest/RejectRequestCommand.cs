

using TechAssessment.Application.Common.Models;

namespace TechAssessment.Application.SponsorshipRequests.Commands.RejectRequest;

public record RejectRequestCommand(int Id, string Remarks, string ApproverRole) : IRequest<int>;
