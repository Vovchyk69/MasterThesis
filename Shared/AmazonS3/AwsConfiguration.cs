namespace Shared.AmazonS3;

public class AwsConfiguration: IAwsConfiguration
{
    public string AwsAccessKey { get; set; }
    public string AwsSecretAccessKey { get; set; }
    public string BucketName { get; set; }
    public string Region { get; set; }

    public AwsConfiguration()
    {
        AwsAccessKey = string.Empty;
        AwsSecretAccessKey = string.Empty;
        BucketName = string.Empty;
        Region = string.Empty;
    }

    public AwsConfiguration(
        string accessKey, 
        string awsSecretAccessKey,
        string bucketName, 
        string region)
    {
        AwsAccessKey = accessKey;
        AwsSecretAccessKey = awsSecretAccessKey;
        BucketName = bucketName;
        Region = region;
    }
}