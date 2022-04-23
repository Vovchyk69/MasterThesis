using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Shared.RabbitMq.Connection;
using Shared.RabbitMq.EventBus;
using Shared.RabbitMq.EventBus.Bus;
using Shared.RabbitMq.EventBus.Events;

namespace Shared.RabbitMq;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

public class RabbitMqEventBus : IEventBus
{
	private readonly string _exchangeName;
	private readonly string _queueName;
	private readonly int _publishRetryCount = 5;
	private readonly TimeSpan _subscribeRetryTime = TimeSpan.FromSeconds(5);

	private readonly IRConnection _connection;
	private readonly ISubscribeManager _subscriptionsManager;
	private readonly IServiceProvider _serviceProvider;

	private IModel _consumerChannel;

	public RabbitMqEventBus(
		IRConnection connection,
		ISubscribeManager subscriptionsManager,
		IServiceProvider serviceProvider,
		string brokerName,
		string queueName)
	{
		_connection = connection ?? throw new ArgumentNullException(nameof(connection));
		_subscriptionsManager = subscriptionsManager ?? throw new ArgumentNullException(nameof(subscriptionsManager));
		_serviceProvider = serviceProvider;
		_exchangeName = brokerName ?? throw new ArgumentNullException(nameof(brokerName));
		_queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));

		ConfigureMessageBroker();
	}

	public void Publish<TEvent>(TEvent @event) where TEvent : Event
	{
		if (!_connection.IsConnected()) _connection.TryConnect();
		
		var eventName = @event.GetType().Name;

		using (var channel = _connection.CreateModel())
		{
			channel.ExchangeDeclare(exchange: _exchangeName, type: "direct");

			var message = JsonSerializer.Serialize(@event);
			var body = Encoding.UTF8.GetBytes(message);

			var properties = channel.CreateBasicProperties();
			properties.DeliveryMode = (byte) DeliveryMode.Persistent;

			channel.BasicPublish(
				exchange: _exchangeName,
				routingKey: eventName,
				mandatory: true,
				body: body);
		}
	}

	public void Subscribe<TEvent, TEventHandler>()
		where TEvent : Event
		where TEventHandler : IEventHandler<TEvent>
	{
		var eventName = _subscriptionsManager.GetEventIdentifier<TEvent>();
		var eventHandlerName = typeof(TEventHandler).Name;

		AddQueueBindForEventSubscription(eventName);

		_subscriptionsManager.AddSubscription<TEvent, TEventHandler>();
		StartBasicConsume();
	}

	public void Unsubscribe<TEvent, TEventHandler>() where TEvent : Event
		where TEventHandler : IEventHandler<TEvent> =>
		_subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();

	private void ConfigureMessageBroker()
	{
		_consumerChannel = CreateConsumerChannel();
		_subscriptionsManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
		_connection.ReconnectOnConnectionFailure += PersistentConnection_OnReconnectedAfterConnectionFailure;
	}

	private IModel CreateConsumerChannel()
	{
		if (!_connection.IsConnected()) _connection.TryConnect();

		var channel = _connection.CreateModel();

		channel.ExchangeDeclare(exchange: _exchangeName, type: "direct");
		channel.QueueDeclare
		(
			queue: _queueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);

		channel.CallbackException += (sender, ea) => DoCreateConsumerChannel();
		return channel;
	}

	private void StartBasicConsume()
	{
		if (_consumerChannel == null) return;

		var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
		consumer.Received += Consumer_Received;

		_consumerChannel.BasicConsume
		(
			queue: _queueName,
			autoAck: false,
			consumer: consumer
		);
	}

	private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
	{
		var eventName = eventArgs.RoutingKey;
		var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

		bool isAcknowledged = false;

		try
		{
			await ProcessEvent(eventName, message);

			_consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
			isAcknowledged = true;
		}
		finally
		{
			if (!isAcknowledged) await TryEnqueueMessageAgainAsync(eventArgs);
		}
	}

	private async Task TryEnqueueMessageAgainAsync(BasicDeliverEventArgs eventArgs)
	{
		try
		{

			await Task.Delay(_subscribeRetryTime);
			_consumerChannel.BasicNack(eventArgs.DeliveryTag, false, true);
		}
		catch (Exception ex)
		{
		}
	}

	private async Task ProcessEvent(string eventName, string message)
	{
		if (!_subscriptionsManager.HasSubscriptionsForEvent(eventName)) return;

		var subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);
		foreach (var subscription in subscriptions)
		{
			var handler = _serviceProvider.GetService(subscription.HandlerType);
			if (handler == null) continue;

			var eventType = _subscriptionsManager.GetEventTypeByName(eventName);

			var @event = JsonSerializer.Deserialize(message, eventType);
			var eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
			await Task.Yield();
			await (Task)eventHandlerType.GetMethod(nameof(IEventHandler<Event>.HandleAsync)).Invoke(handler, new object[] { @event });
		}
	}

	private void SubscriptionManager_OnEventRemoved(object sender, string eventName)
	{
		if (!_connection.IsConnected()) _connection.TryConnect();

		using var channel = _connection.CreateModel();
		channel.QueueUnbind(queue: _queueName, exchange: _exchangeName, routingKey: eventName);

		if (_subscriptionsManager.IsEmpty)
		{
			_consumerChannel.Close();
		}
	}

	private void AddQueueBindForEventSubscription(string eventName)
	{
		var containsKey = _subscriptionsManager.HasSubscriptionsForEvent(eventName);
		if (containsKey) return;

		if (!_connection.IsConnected()) _connection.TryConnect();

		using var channel = _connection.CreateModel();
		channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: eventName);
	}

	private void PersistentConnection_OnReconnectedAfterConnectionFailure(object sender, EventArgs e)
	{
		DoCreateConsumerChannel();
		RecreateSubscriptions();
	}

	private void DoCreateConsumerChannel()
	{
		_consumerChannel.Dispose();
		_consumerChannel = CreateConsumerChannel();
		StartBasicConsume();
	}

	private void RecreateSubscriptions()
	{
		var subscriptions = _subscriptionsManager.GetAllSubscriptions();
		_subscriptionsManager.Clear();

		var eventBusType = this.GetType();

		foreach (var entry in subscriptions)
		{
			foreach (var subscription in entry.Value)
			{
				var genericSubscribe = eventBusType.GetMethod( "Subscribe")!.MakeGenericMethod(subscription.EventType, subscription.HandlerType);
				genericSubscribe.Invoke(this, null);
			}
		}
	}
}