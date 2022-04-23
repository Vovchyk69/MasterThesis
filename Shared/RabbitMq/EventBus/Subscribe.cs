namespace Shared.RabbitMq.EventBus;

public class Subscribe
{
    public Type EventType { get; }
    
    public Type HandlerType { get; }

    public Subscribe(Type eventType, Type handlerType)
    {
        EventType = eventType;
        HandlerType = handlerType;
    }
}