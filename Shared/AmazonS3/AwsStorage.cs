using System.Linq.Expressions;
using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace Shared.AmazonS3;

public class AwsStorage: IAwsStorage
{
    private readonly string _bucketName;
    private readonly IAmazonS3 _awsS3Client;

    public AwsStorage(string awsAccessKeyId, string awsSecretAccessKey, string region, string bucketName)
    {
        _bucketName = bucketName;
        _awsS3Client = new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.GetBySystemName(region));

    }
    
    public async Task<bool> UploadFileAsync(IFormFile file)
    {
        try
        {
            await using var newMemoryStream = new MemoryStream();
            await file.CopyToAsync(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = newMemoryStream,
                Key = file.FileName,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var fileTransferUtility = new TransferUtility(_awsS3Client);

            await fileTransferUtility.UploadAsync(uploadRequest);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not upload file");
            return false;
        }
    }

    public async Task DeleteFileAsync(string fileName, string versionId)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName
        };

        if (!string.IsNullOrEmpty(versionId))
            request.VersionId = versionId;

        await _awsS3Client.DeleteObjectAsync(request);
    }
    
    public async Task<List<S3Object>> GetFilesAsync()
    {
        var request = new ListObjectsRequest
        {
            BucketName = _bucketName
        };
        
        var result = await _awsS3Client.ListObjectsAsync(request);
        return result.S3Objects;
    }

    public async Task<string> DownloadFileAsync(string file)
    {
        StreamReader? reader = null;
        try
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = file
            };

            var response = await _awsS3Client.GetObjectAsync(getObjectRequest);
            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                using (reader = new StreamReader(response.ResponseStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }

            if (reader is null)
                throw new FileNotFoundException($"The document '{file}' is not found");

            return await reader.ReadToEndAsync();

        }
        catch (Exception ex)
        {
            Console.WriteLine("System error... Please try again");
            throw ex;
        }
    }
}