using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Shared.RabbitMq.Connection;
using Shared.RabbitMq.EventBus;
using Shared.RabbitMq.EventBus.Bus;

namespace Shared.RabbitMq;

public static class ServiceCollectionExtensions
{
      public static void AddRabbitMQEventBus(this IServiceCollection services, string connectionUrl, string brokerName, string queueName, int timeoutBeforeReconnecting = 15)
    {
        services.AddSingleton<ISubscribeManager, EventSubscribeManager>();
        services.AddSingleton<IRConnection, RabbitMqConnection>(factory =>
        {
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connectionUrl),
                DispatchConsumersAsync = true,
            };

            return new RabbitMqConnection(connectionFactory, timeoutBeforeReconnecting);
        });

        services.AddSingleton<IEventBus, RabbitMqEventBus>(factory =>
        {
            var persistentConnection = factory.GetService<IRConnection>();
            var subscriptionManager = factory.GetService<ISubscribeManager>();

            return new RabbitMqEventBus(persistentConnection, subscriptionManager, factory, brokerName, queueName);
        });
    }
}