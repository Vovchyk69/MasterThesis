using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Shared.AmazonS3;
using Shared.RabbitMq.EventBus.Events;
using Shared.RabbitMq.Events;

namespace Shared.RabbitMq.EventHandlers;

public class FileUploadHandler : IEventHandler<FileUploadMessage>
{
    private readonly IAwsConfiguration _configuration;
    private readonly IAwsStorage _awsStorage;

    public FileUploadHandler(IAwsConfiguration configuration)
    {
        _configuration = configuration;
        _awsStorage = new AwsStorage(
            _configuration.AwsAccessKey, 
            _configuration.AwsSecretAccessKey, 
            _configuration.Region,
            _configuration.BucketName);
    }

    public async Task HandleAsync(FileUploadMessage @event)
    {
        var file = await _awsStorage.DownloadFileAsync(@event.FileName);
    }
}