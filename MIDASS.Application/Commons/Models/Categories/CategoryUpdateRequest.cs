
using FluentValidation;
using MIDASS.Contract.Constrants;
using MIDASS.Contract.Messages.Validations;
using MIDASS.Domain.Constrants;

namespace MIDASS.Application.Commons.Models.Categories;

public class CategoryUpdateRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
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
            .NotEmpty().WithMessage(CategoryValidationMessages.CategoryNameShouldNotBeEmpty)
            .Must(name => name.Length <= CategoryValidationRules.MaxLengthCategoryName)
            .WithMessage(string.Format(CategoryValidationMessages.CategoryNameShouldLessThanOrEqualMaxLength, CategoryValidationRules.MaxLengthCategoryName));
        RuleFor(c => c.Description)
            .Must(description => description == null || description.Length <= CategoryValidationRules.MaxLengthCategoryDescription)
            .WithMessage(string.Format(CategoryValidationMessages.CategoryDescriptionShouldLessThanOrEqualMaxLength, CategoryValidationRules.MaxLengthCategoryDescription));
    }
}
