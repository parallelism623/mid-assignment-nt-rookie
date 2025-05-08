using Microsoft.AspNetCore.Http;

namespace MIDASM.Application.Services.FileServices;

public interface IImageStorageServices
{
    Task<string> UploadImageAsync(IFormFile imageUploadRequset, CancellationToken token = default);
    Task<string> GetPreSignedUrlImage(string imageKey);
    Task DeleteImageAsync(string imageDeleteRequest, CancellationToken token = default);
}
