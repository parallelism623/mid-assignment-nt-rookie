using FluentValidation;
using MIDASS.Contract.Messages.Validations;
using MIDASS.Domain.Entities;

namespace MIDASS.Application.Commons.Models.Books;

public class BookUpdateRequest
{
    public Guid Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Author { get; set; } = default!;

    public int AddedQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public List<string>? SubImagesUrl { get; set; }
    public Guid CategoryId { get; set; }
}


public class BookUpdateRequestValidator : AbstractValidator<BookUpdateRequest>
{
    public BookUpdateRequestValidator()
    {
        RuleFor(b => b.Id).NotEmpty().WithMessage(BookValidationMessages.IdShouldNotBeEmpty);
        RuleFor(b => b.Title)
            .NotEmpty().WithMessage(BookValidationMessages.TitleShouldNotBeEmpty)
            .MaximumLength(100).WithMessage(BookValidationMessages.TitleShouldLessEqualThanMaxLength);
        RuleFor(b => b.Description)
            .MaximumLength(2000).WithMessage(BookValidationMessages.DescriptionShouldLessEqualThanMaxLength);
        RuleFor(b => b.Author)
            .NotEmpty().WithMessage(BookValidationMessages.AuthorShouldNotBeEmpty)
            .MaximumLength(100).WithMessage(BookValidationMessages.AuthorShouldLessEqualThanMaxLength);
 
    }
}