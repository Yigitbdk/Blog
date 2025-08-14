using BlogApp.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Validators
{
    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("Invalid post ID");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Invalid user ID");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .Length(1, 1000).WithMessage("Comment content must be between 1 and 1000 characters")
                .Must(content => !string.IsNullOrWhiteSpace(content?.Trim()))
                .WithMessage("Comment cannot be empty or whitespace only");
        }
    }
}
