using RabbitMQ.Client;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.IO;
using System.Net.Sockets;

namespace Shared.RabbitMq.Connection;

public class RabbitMqConnection: IRConnection
{
    private readonly object _locker = new object();
    
    private readonly IConnectionFactory _connectionFactory;
    private readonly TimeSpan _reconnectTimeOut;
    
    private IConnection _connection;
    private bool _connectionFailed;
    
    public event EventHandler? ReconnectOnConnectionFailure;
    
    public bool IsConnected() =>
        _connection is {IsOpen: true}; 

    public RabbitMqConnection(IConnectionFactory factory, int timeOut = 10)
    {
        _connectionFactory = factory;
        _reconnectTimeOut = TimeSpan.FromSeconds(timeOut);;
    }
    
    public bool TryConnect()
    {
        lock (_locker)
        {
            var policy = Policy
                .Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetryForever((duration) => _reconnectTimeOut);

            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();
            });

            if (!IsConnected())
            {
                _connectionFailed = true;
                return false;
            }

            if (!_connectionFailed) return true;
            
            ReconnectOnConnectionFailure?.Invoke(this, null);
            _connectionFailed = false;

            return true;
        }
    }

    public IModel CreateModel()
    {
        if (!IsConnected())
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action.");

        return _connection.CreateModel();
    }
}