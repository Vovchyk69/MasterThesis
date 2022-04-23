using RabbitMQ.Client;

namespace Shared.RabbitMq.Connection;

public interface IRConnection
{
    event EventHandler ReconnectOnConnectionFailure;

    bool IsConnected();

    bool TryConnect();

    IModel CreateModel();
}