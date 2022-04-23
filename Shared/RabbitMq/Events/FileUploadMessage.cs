using Shared.RabbitMq.EventBus.Events;

namespace Shared.RabbitMq.Events;

public class FileUploadMessage: Event
{
    public string Message { get; set; }

    public FileUploadMessage(string message)
    {
        Message = message;
    }

    public override string ToString() => $"The message was created on - {CreatedOn}";
}