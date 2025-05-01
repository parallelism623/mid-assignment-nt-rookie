using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MIDASS.Application.Services.FileServices;
using MIDASS.Infrastructure.Options;

namespace MIDASS.Infrastructure.Files;

public class ImageStorageServices : IImageStorageServices
{
    private readonly IAmazonS3 _awss3Client;
    private readonly AWSS3Options _awss3Options;
    private readonly ILogger<ImageStorageServices> _logger;
    public ImageStorageServices(IAmazonS3 awss3Client, 
        IOptions<AWSS3Options> awss3Options,
        ILogger<ImageStorageServices> logger)
    {
        _logger = logger;
        _awss3Client = awss3Client;
        _awss3Options = awss3Options.Value;
    }
    public async Task<string> UploadImageAsync(IFormFile imageUploadRequset, CancellationToken token = default)
    {
        var formFile = imageUploadRequset;
        var imagePath = Path.GetExtension(formFile.FileName);
        try
        {
            var stream = formFile.OpenReadStream();
            
            var key = $"{Guid.NewGuid()}-{imagePath}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                BucketName = _awss3Options.BucketName,
                Key = key,
                InputStream = stream,
                ContentType = formFile.ContentType,
                AutoCloseStream = true
            };

            var transferUtil = new TransferUtility(_awss3Client);
            await transferUtil.UploadAsync(uploadRequest, token);

            return key;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Can not upload image by aws3  \n Path : {Path} \n Time upload: {Time} \n", imagePath, DateTime.UtcNow);
            return string.Empty;
        }
    }
    public Task<string> GetPreSignedUrlImage(string imageKey)
    {
        try
        {
            var preReq = new GetPreSignedUrlRequest
            {
                BucketName = _awss3Options.BucketName,
                Key = imageKey,
                Expires = DateTime.UtcNow.AddHours(1),
                Verb = HttpVerb.GET
            };
            return _awss3Client.GetPreSignedURLAsync(preReq);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Task.FromResult(string.Empty);
        }
    }

    public async Task  DeleteImageAsync(string imageDeleteRequest, CancellationToken token = default)
    {
        try
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _awss3Options.BucketName,
                Key = imageDeleteRequest,
            };

  
            await _awss3Client.DeleteObjectAsync(deleteObjectRequest, token);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex, "Can not upload image by aws3  \n Path : {Path} \n Time upload: {Time} \n", imageDeleteRequest, DateTime.UtcNow);
        }
    }

    public async Task<string> GetSignedUrlImageAsync(string imageKey)
    {
        return await GetPreSignedUrlImage(imageKey);
    }
}
