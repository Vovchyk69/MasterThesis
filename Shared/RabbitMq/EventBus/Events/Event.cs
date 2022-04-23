namespace Shared.RabbitMq.EventBus.Events;

public abstract class Event
{
    public Guid Id => Guid.NewGuid();
    public DateTime CreatedOn => DateTime.Now;
}