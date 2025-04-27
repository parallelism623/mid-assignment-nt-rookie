
using FluentValidation;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Categories;

public class CategoryUpdateRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; } = default!;
}


public class UpdateCategoryRequestValidator : AbstractValidator<CategoryUpdateRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage(CategoryValidationMessages.CategoryIdShouldNotBeEmpty);
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage(CategoryValidationMessages.CategoryNameShouldNotBeEmpty);
    }
}
