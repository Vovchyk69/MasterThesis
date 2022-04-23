using Shared.RabbitMq.EventBus.Events;
using Shared.RabbitMq.Events;

public class FileUploadHandler : IEventHandler<FileUploadMessage>
{
    public FileUploadHandler()
    {
    }

    public Task HandleAsync(FileUploadMessage @event)
    {
        return Task.CompletedTask;    
    }
}