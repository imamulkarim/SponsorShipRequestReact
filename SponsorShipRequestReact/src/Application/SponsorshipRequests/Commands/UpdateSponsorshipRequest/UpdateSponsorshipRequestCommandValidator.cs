using FluentValidation;

namespace TechAssessment.Application.SponsorshipRequests.Commands.UpdateSponsorshipRequest;

public class UpdateSponsorshipRequestCommandValidator : AbstractValidator<UpdateSponsorshipRequestCommand>
{
    public UpdateSponsorshipRequestCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("ID must be greater than 0.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.");

        RuleFor(x => x.SponsorshipTypeId)
            .GreaterThan(0).WithMessage("Sponsorship Type is required.");

        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("Event Name is required.");

        RuleFor(x => x.EventDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddDays(-1))
            .WithMessage("Event Date must be today or in the future.");

        RuleFor(x => x.RequestedAmount)
            .GreaterThan(0).WithMessage("Requested Amount must be greater than 0.");

        RuleFor(x => x.Purpose)
            .NotEmpty().WithMessage("Purpose is required.")
            .MinimumLength(10).WithMessage("Purpose must be at least 10 characters.");

        RuleFor(x => x.BusinessBenefit)
            .MaximumLength(2000).WithMessage("Business Benefit must not exceed 2000 characters.");

        RuleFor(x => x.SupportingDocumentUrl)
            .Must(x => string.IsNullOrEmpty(x) || Uri.TryCreate(x, UriKind.Absolute, out _))
            .WithMessage("Supporting Document URL must be a valid URL.");
    }
}
