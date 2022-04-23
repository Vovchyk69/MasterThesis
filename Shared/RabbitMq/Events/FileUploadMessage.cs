using Shared.RabbitMq.EventBus.Events;

namespace Shared.RabbitMq.Events;

public class FileUploadMessage: Event
{
    public string Message { get; set; }

    public override string ToString() => $"The message was created on - {CreatedOn}";
}