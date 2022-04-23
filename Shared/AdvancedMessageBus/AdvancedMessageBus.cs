namespace Shared.AdvancedMessageBus;

public class AdvancedMessageBus
{
    
}

public interface IMessageBus
{
    string Driver { get; }
    
    void Initialize(string arguments);
    
    Task Subscribe(string subscriptionId, Type messageType);
}