using FluentValidation;
using AnnounceService.Application.DTOs.Comments;

namespace AnnounceService.Application.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequestDto>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.AnnounceId)
            .NotEmpty().WithMessage("Announce ID is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
    }
}