
using FluentValidation;
using MIDASS.Contract.Messages.Validations;

namespace MIDASS.Application.Commons.Models.Files;

public class ImageDeleteRequest
{
    public string ImagePath { get; set; } = default!;
}


public class ImageDeleteRequestValidator : AbstractValidator<ImageDeleteRequest>
{
    public ImageDeleteRequestValidator()
    {
        RuleFor(r => r.ImagePath)
            .NotEmpty()
            .WithMessage(FileValidationMessages.FilePathMustBeNotEmpty);
    }
}

