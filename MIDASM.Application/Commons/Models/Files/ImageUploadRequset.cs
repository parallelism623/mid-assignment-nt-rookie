
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace MIDASM.Application.Commons.Models.Files;

public class ImageUploadRequset
{
    public IFormFile Image { get; set; } = default!;
}

