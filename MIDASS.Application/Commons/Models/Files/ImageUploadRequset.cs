
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace MIDASS.Application.Commons.Models.Files;

public class ImageUploadRequset
{
    public IFormFile Image { get; set; } = default!;
}

