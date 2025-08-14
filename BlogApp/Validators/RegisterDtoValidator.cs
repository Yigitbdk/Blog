using BlogApp.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 50).WithMessage("Username must be between 3 and 50 characters")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(6, 100).WithMessage("Password must be between 6 and 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one digit");

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Bio));
        }
    }
}
