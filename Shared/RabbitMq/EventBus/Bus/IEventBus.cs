﻿using Shared.RabbitMq.EventBus.Events;

namespace Shared.RabbitMq.EventBus.Bus;

public interface IEventBus
{
    void Publish<TEvent>(TEvent @event) where TEvent : Event;

    void Subscribe<TEvent, TEventHandler>() where TEvent : Event
        where TEventHandler : IEventHandler<TEvent>;

    void Unsubscribe<TEvent, TEventHandler>() where TEvent : Event 
        where TEventHandler : IEventHandler<TEvent>;
}