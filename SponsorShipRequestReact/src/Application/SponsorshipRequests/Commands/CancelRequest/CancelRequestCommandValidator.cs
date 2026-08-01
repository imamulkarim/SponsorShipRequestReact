using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Application.SponsorshipRequests.Commands.CancelRequest;

public class CancelRequestCommandValidator : AbstractValidator<CancelRequestCommand>
{
    public CancelRequestCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Valid request ID is required.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Cancellation reason is required.");
    }
}
