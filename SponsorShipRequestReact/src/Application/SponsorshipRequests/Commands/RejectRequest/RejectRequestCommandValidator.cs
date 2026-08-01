using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Application.SponsorshipRequests.Commands.RejectRequest;

public class RejectRequestCommandValidator : AbstractValidator<RejectRequestCommand>
{
    public RejectRequestCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Valid request ID is required.");

        RuleFor(x => x.Remarks)
            .NotEmpty().WithMessage("Rejection remarks are required.");
    }
}
