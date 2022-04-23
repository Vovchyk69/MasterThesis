namespace Shared.AmazonS3;

public interface IAwsConfiguration
{
    string AwsAccessKey { get; set; }
    
    string AwsSecretAccessKey { get; set; }
    
    string BucketName { get; set; }
    
    string Region { get; set; }
}