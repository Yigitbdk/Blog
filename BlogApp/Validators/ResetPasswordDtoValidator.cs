using BlogApp.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Validators
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token is required")
                .MinimumLength(10).WithMessage("Invalid reset token format");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .Length(6, 100).WithMessage("Password must be between 6 and 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one digit");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Password confirmation is required")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
        }
    }
}
