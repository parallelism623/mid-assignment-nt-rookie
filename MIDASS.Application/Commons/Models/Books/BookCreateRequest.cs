
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Books;

public class BookCreateRequest
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Author { get; set; } = default!;
    public int Quantity { get; set; }
    public int Available { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? SubImagesUrl { get; set; }
    public Guid CategoryId { get; set; }
}

public class BookCreateRequestValidator : AbstractValidator<BookCreateRequest>
{
    public BookCreateRequestValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty().WithMessage(BookValidationMessages.TitleShouldNotBeEmpty)
            .MaximumLength(100).WithMessage(BookValidationMessages.TitleShouldLessEqualThanMaxLength);
        RuleFor(b => b.Description)
            .MaximumLength(2000).WithMessage(BookValidationMessages.DescriptionShouldLessEqualThanMaxLength);
        RuleFor(b => b.Author)
            .NotEmpty().WithMessage(BookValidationMessages.AuthorShouldNotBeEmpty)
            .MaximumLength(100).WithMessage(BookValidationMessages.AuthorShouldLessEqualThanMaxLength);
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