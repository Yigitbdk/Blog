using BlogApp.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Validators
{
    public class PostDtoValidator : AbstractValidator<PostDto>
    {
        public PostDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Post title is required")
                .Length(5, 200).WithMessage("Post title must be between 5 and 200 characters")
                .Must(title => !string.IsNullOrWhiteSpace(title?.Trim()))
                .WithMessage("Post title cannot be empty or whitespace only");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Post content is required")
                .Length(10, 5000).WithMessage("Post content must be between 10 and 5000 characters")
                .Must(content => !string.IsNullOrWhiteSpace(content?.Trim()))
                .WithMessage("Post content cannot be empty or whitespace only");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Invalid user ID");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Invalid category ID")
                .When(x => x.CategoryId.HasValue);
        }
    }
}
