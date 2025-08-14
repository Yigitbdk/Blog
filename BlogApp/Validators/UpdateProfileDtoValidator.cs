using BlogApp.Dto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Validators
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.Bio)
                .MaximumLength(1000).WithMessage("Bio cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Bio));

            RuleFor(x => x.ProfilePicture)
                .Must(BeAValidUrl).WithMessage("Invalid profile picture URL")
                .MaximumLength(500).WithMessage("Profile picture URL cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.ProfilePicture));
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
