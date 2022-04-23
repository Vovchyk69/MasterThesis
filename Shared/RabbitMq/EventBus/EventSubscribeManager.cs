using Shared.RabbitMq.EventBus;
using Shared.RabbitMq.EventBus.Events;

public class EventSubscribeManager : ISubscribeManager
	{
		private readonly Dictionary<string, List<Subscribe>> _handlers = new Dictionary<string, List<Subscribe>>();
		private readonly List<Type> _eventTypes = new List<Type>();

		public event EventHandler<string> OnEventRemoved;

		public string GetEventIdentifier<TEvent>() => typeof(TEvent).Name;

		public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(t => t.Name == eventName);

		public IEnumerable<Subscribe> GetHandlersForEvent(string eventName) => _handlers[eventName];

		public Dictionary<string, List<Subscribe>> GetAllSubscriptions() => new (_handlers);

		public void AddSubscription<TEvent, TEventHandler>() where TEvent : Event
			where TEventHandler : IEventHandler<TEvent>
		{
			var eventName = GetEventIdentifier<TEvent>();

			DoAddSubscription(typeof(TEvent), typeof(TEventHandler), eventName);

			if (!_eventTypes.Contains(typeof(TEvent)))
			{
				_eventTypes.Add(typeof(TEvent));
			}
		}

		public void RemoveSubscription<TEvent, TEventHandler>()
			where TEventHandler : IEventHandler<TEvent>
			where TEvent : Event
		{
			var handlerToRemove = FindSubscriptionToRemove<TEvent, TEventHandler>();
			var eventName = GetEventIdentifier<TEvent>();
			DoRemoveHandler(eventName, handlerToRemove);
		}

		public void Clear()
		{
			_handlers.Clear();
			_eventTypes.Clear();
		}

		public bool IsEmpty => !_handlers.Keys.Any();

		public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

		private void DoAddSubscription(Type eventType, Type handlerType, string eventName)
		{
			if (!HasSubscriptionsForEvent(eventName))
			{
				_handlers.Add(eventName, new List<Subscribe>());
			}

			if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
			{
				throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
			}

			_handlers[eventName].Add(new Subscribe(eventType, handlerType));
		}

		private void DoRemoveHandler(string eventName, Subscribe subscriptionToRemove)
		{
			if (subscriptionToRemove == null)
			{
				return;
			}

			_handlers[eventName].Remove(subscriptionToRemove);
			if (_handlers[eventName].Any())
			{
				return;
			}

			_handlers.Remove(eventName);
			var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
			if (eventType != null)
			{
				_eventTypes.Remove(eventType);
			}

			RaiseOnEventRemoved(eventName);
		}

		private void RaiseOnEventRemoved(string eventName)
		{
			var handler = OnEventRemoved;
			handler?.Invoke(this, eventName);
		}

		private Subscribe FindSubscriptionToRemove<TEvent, TEventHandler>() where TEvent : Event
			 where TEventHandler : IEventHandler<TEvent>
		{
			var eventName = GetEventIdentifier<TEvent>();
			return DoFindSubscriptionToRemove(eventName, typeof(TEventHandler));
		}

		private Subscribe DoFindSubscriptionToRemove(string eventName, Type handlerType)
		{
			return !HasSubscriptionsForEvent(eventName) ? null : _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
		}
	}