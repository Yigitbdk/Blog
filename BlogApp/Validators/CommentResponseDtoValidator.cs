using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Validators
{
    public class CommentResponseDtoValidator : AbstractValidator<CommentResponseDto>
    {
        public CommentResponseDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid comment ID");

            RuleFor(x => x.PostId)
                .GreaterThan(0).WithMessage("Invalid post ID");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Invalid user ID");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required")
                .Length(1, 1000).WithMessage("Comment content must be between 1 and 1000 characters");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .Length(2, 50).WithMessage("Username must be between 2 and 50 characters");

            RuleFor(x => x.CreateDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Create date cannot be in the future");
        }
    }
}
