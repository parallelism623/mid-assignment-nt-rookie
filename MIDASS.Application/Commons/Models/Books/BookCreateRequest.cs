
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MIDASS.Contract.Messages.Validations;
using MIDASS.Domain.Constrants;

namespace MIDASS.Application.Commons.Models.Books;

public class BookCreateRequest
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Author { get; set; } = default!;
    public int Quantity { get; set; }
    public int Available { get; set; }
    public IFormFile? ImageUrl { get; set; }
    public List<IFormFile>? SubImagesUrl { get; set; }
    public Guid CategoryId { get; set; }
}

public class BookCreateRequestValidator : AbstractValidator<BookCreateRequest>
{
    public BookCreateRequestValidator()
    {
        RuleFor(b => b.CategoryId).NotEmpty()
            .WithMessage(BookValidationMessages.BookMustHaveCategory);
        RuleFor(b => b.Title)
            .NotEmpty().WithMessage(BookValidationMessages.TitleShouldNotBeEmpty)
            .MaximumLength(BookValidationRules.MaxLengthTitle)
            .WithMessage(string.Format(BookValidationMessages.TitleShouldLessEqualThanMaxLength, BookValidationRules.MaxLengthTitle));
        RuleFor(b => b.Description)
            .MaximumLength(BookValidationRules.MaxLengthDescription)
            .WithMessage(string.Format(BookValidationMessages.DescriptionShouldLessEqualThanMaxLength, BookValidationRules.MaxLengthDescription));
        RuleFor(b => b.Author)
            .NotEmpty().WithMessage(BookValidationMessages.AuthorShouldNotBeEmpty)
            .MaximumLength(BookValidationRules.MaxLengthAuthor)
            .WithMessage(string.Format(BookValidationMessages.AuthorShouldLessEqualThanMaxLength, BookValidationRules.MaxLengthAuthor));
        RuleFor(b => b.Available).LessThanOrEqualTo(p => p.Quantity)
            .WithMessage(BookValidationMessages.BookAvailableShouldLessThanOrEqualQuantity);
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BookValidationMessages.BookQuantityShouldGreaterThanZero);
        RuleFor(x => x.Available)
            .GreaterThanOrEqualTo(0)
            .WithMessage(BookValidationMessages.BookAvailableShouldGreaterThanZero);
    }
}