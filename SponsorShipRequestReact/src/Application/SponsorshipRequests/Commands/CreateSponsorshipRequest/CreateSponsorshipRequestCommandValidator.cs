using System;
using System.Collections.Generic;
using System.Text;

namespace TechAssessment.Application.SponsorshipRequests.Commands.CreateSponsorshipRequest;

public class CreateSponsorshipRequestCommandValidator : AbstractValidator<CreateSponsorshipRequestCommand>
{
    public CreateSponsorshipRequestCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.");

        RuleFor(x => x.SponsorshipTypeId)
            .GreaterThan(0).WithMessage("Valid sponsorship type is required.");

        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("Event name is required.");

        RuleFor(x => x.EventDate)
            .GreaterThan(DateTime.Now).WithMessage("Event date must be in the future.");

        RuleFor(x => x.RequestedAmount)
            .GreaterThan(0).WithMessage("Requested amount must be greater than 0.");

        RuleFor(x => x.Purpose)
            .NotEmpty().WithMessage("Purpose is required.")
            .MinimumLength(10).WithMessage("Purpose must be at least 10 characters.");
    }
}
