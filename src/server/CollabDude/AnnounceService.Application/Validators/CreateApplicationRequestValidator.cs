using FluentValidation;
using AnnounceService.Application.DTOs.Application;

namespace AnnounceService.Application.Validators;

public class CreateApplicationRequestValidator : AbstractValidator<CreateApplicationRequestDto>
{
    public CreateApplicationRequestValidator()
    {
        RuleFor(x => x.AnnounceId)
            .NotEmpty().WithMessage("Announce ID is required");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Application message is required")
            .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters");
    }
}