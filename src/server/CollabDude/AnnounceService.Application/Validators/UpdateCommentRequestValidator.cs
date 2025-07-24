using FluentValidation;
using AnnounceService.Application.DTOs.Comments;

namespace AnnounceService.Application.Validators;

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequestDto>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Comment ID is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
    }
}