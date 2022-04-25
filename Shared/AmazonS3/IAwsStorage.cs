using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace Shared.AmazonS3;

public interface IAwsStorage
{
    Task<string> DownloadFileAsync(string file);

    Task<bool> UploadFileAsync(IFormFile file);

    Task DeleteFileAsync(string fileName, string versionId = "");
    
    Task<List<S3Object>> GetFilesAsync();

}