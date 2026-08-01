using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Application.SponsorshipRequests.Commands.SubmitSponsorshipRequest;

public class SubmitSponsorshipRequestCommandValidator : AbstractValidator<SubmitSponsorshipRequestCommand>
{
    public SubmitSponsorshipRequestCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Valid request ID is required.");
    }
}
