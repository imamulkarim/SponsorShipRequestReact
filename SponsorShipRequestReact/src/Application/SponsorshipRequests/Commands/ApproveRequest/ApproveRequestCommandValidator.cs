

namespace TechAssessment.Application.SponsorshipRequests.Commands.ApproveRequest;

public class ApproveRequestCommandValidator : AbstractValidator<ApproveRequestCommand>
{
    public ApproveRequestCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Valid request ID is required.");

        RuleFor(x => x.Remarks)
            .NotEmpty().WithMessage("Approval remarks are required.");

        RuleFor(x => x.ApproverRole)
            .NotEmpty().WithMessage("Approver role is required.");
    }
}
