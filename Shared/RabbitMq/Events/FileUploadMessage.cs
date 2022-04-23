using Shared.RabbitMq.EventBus.Events;

namespace Shared.RabbitMq.Events;

public class FileUploadMessage: Event
{
    public string FileName { get; set; }

    public FileUploadMessage(string fileName)
    {
        FileName = fileName;
    }

    public override string ToString() => $"The message was created on - {CreatedOn}";
}