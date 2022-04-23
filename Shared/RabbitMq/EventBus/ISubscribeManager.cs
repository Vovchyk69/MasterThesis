using Shared.RabbitMq.EventBus.Events;

namespace Shared.RabbitMq.EventBus;

public interface ISubscribeManager
{
    event EventHandler<string> OnEventRemoved;

    bool IsEmpty { get; }
    
    bool HasSubscriptionsForEvent(string eventName);

    string GetEventIdentifier<TEvent>();
    
    Type GetEventTypeByName(string eventName);
    
    IEnumerable<Subscribe> GetHandlersForEvent(string eventName);
    
    Dictionary<string, List<Subscribe>> GetAllSubscriptions();

    void AddSubscription<TEvent, TEventHandler>() where TEvent : Event
        where TEventHandler : IEventHandler<TEvent>;

    void RemoveSubscription<TEvent, TEventHandler>() where TEvent : Event
        where TEventHandler : IEventHandler<TEvent>;

    void Clear();
}