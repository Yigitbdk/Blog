using BlogApp.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlogApp.Validators
{
    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one digit");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Password confirmation is required")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match");

            RuleFor(x => x.NewPassword)
                .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password");
        }
    }
}
