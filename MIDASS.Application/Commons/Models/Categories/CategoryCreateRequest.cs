
using FluentValidation;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Categories;

public class CategoryCreateRequest
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}

public class CreateCategoryRequestValidator : AbstractValidator<CategoryCreateRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage(CategoryValidationMessages.CategoryNameShouldNotBeEmpty);

    }
}
