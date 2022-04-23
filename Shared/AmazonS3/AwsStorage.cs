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

    public async Task<byte[]> DownloadFileAsync(string file)
    {
        MemoryStream? ms = null;

        try
        {
            var getObjectRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = file
            };

            using var response = await _awsS3Client.GetObjectAsync(getObjectRequest);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                await using (ms = new MemoryStream()) ;
                await response.ResponseStream.CopyToAsync(ms);
            }

            if (ms is null || ms.ToArray().Length < 1)
                throw new FileNotFoundException($"The document '{file}' is not found");

            return ms.ToArray();
        }
        catch (Exception ex)
        {
            Console.WriteLine("System error... Please try again");
            throw ex;
        }
    }
}