using FluentValidation;
using AnnounceService.Application.DTOs.Announce;

namespace AnnounceService.Application.Validators;

public class CreateAnnounceRequestValidator : AbstractValidator<CreateAnnounceRequestDto>
{
    public CreateAnnounceRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("Max participants must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Max participants cannot exceed 100");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future")
            .When(x => x.ExpiryDate.HasValue);

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters");

        RuleFor(x => x.RequiredSkills)
            .MaximumLength(1000).WithMessage("Required skills cannot exceed 1000 characters");

        RuleFor(x => x.ContactInfo)
            .MaximumLength(500).WithMessage("Contact info cannot exceed 500 characters");

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 10).WithMessage("Cannot have more than 10 tags")
            .ForEach(tag => tag.MaximumLength(50).WithMessage("Tag name cannot exceed 50 characters"));
    }
}