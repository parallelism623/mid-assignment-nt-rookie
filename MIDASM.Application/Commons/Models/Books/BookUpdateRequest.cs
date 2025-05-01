using FluentValidation;
using Microsoft.AspNetCore.Http;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants;

namespace MIDASM.Application.Commons.Models.Books;

public class BookUpdateRequest
{
    public Guid Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Author { get; set; } = default!;

    public int AddedQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? SubImagesUrl { get; set; }
    public IFormFile? NewImage { get; set; }
    public List<IFormFile>? NewSubImages { get; set; }
    public List<int>? NewSubImagesPos { get; set; }
    public Guid CategoryId { get; set; }
}


public class BookUpdateRequestValidator : AbstractValidator<BookUpdateRequest>
{
    public BookUpdateRequestValidator()
    {
        RuleFor(b => b.Id).NotEmpty().WithMessage(BookValidationMessages.IdShouldNotBeEmpty);
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

    }
}