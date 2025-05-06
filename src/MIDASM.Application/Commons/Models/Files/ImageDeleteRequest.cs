
using FluentValidation;
using MIDASM.Contract.Messages.Validations;

namespace MIDASM.Application.Commons.Models.Files;

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

