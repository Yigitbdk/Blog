using FluentValidation;
using BlogApp.Dto;

namespace BlogApp.Validators;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .Length(2, 100).WithMessage("Category name must be between 2 and 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-_&]+$").WithMessage("Category name contains invalid characters");
    }
}