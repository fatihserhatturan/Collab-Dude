using FluentValidation;
using AnnounceService.Application.DTOs.Tag;

namespace AnnounceService.Application.Validators;

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequestDto>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tag name is required")
            .MaximumLength(50).WithMessage("Tag name cannot exceed 50 characters")
            .Matches("^[a-zA-Z0-9çğıöşüÇĞIİÖŞÜ\\s-]+$").WithMessage("Tag name contains invalid characters");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Tag color is required")
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Invalid color format. Use hex format like #ff0000");
    }
}