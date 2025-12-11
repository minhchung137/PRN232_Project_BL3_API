using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PRN232_GradingSystem_Services.Services.Interfaces;
using RabbitMQ.Client;

namespace PRN232_GradingSystem_Services.Services.Implementations;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly string _exchange;
    private readonly string _routingKey;
    private readonly string _queue;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        var section = configuration.GetSection("RabbitMQ");
        var portValue = section["Port"];
        var vhost = section["VirtualHost"];
        var factory = new ConnectionFactory
        {
            HostName = section["HostName"],
            Port = int.TryParse(portValue, out var p) ? p : 5672,
            VirtualHost = string.IsNullOrWhiteSpace(vhost) ? "/" : vhost,
            UserName = section["UserName"],
            Password = section["Password"],
            DispatchConsumersAsync = true
        };
        _exchange = section["Exchange"] ?? string.Empty;
        _routingKey = section["RoutingKey"] ?? string.Empty;
        _queue = section["Queue"] ?? string.Empty;
        _connection = factory.CreateConnection();
        using var channel = _connection.CreateModel();
        if (!string.IsNullOrWhiteSpace(_queue))
        {
            channel.QueueDeclare(queue: _queue, durable: true, exclusive: false, autoDelete: false);
        }
        if (!string.IsNullOrWhiteSpace(_exchange))
        {
            channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct, durable: true);
            if (!string.IsNullOrWhiteSpace(_queue))
            {
                var rk = string.IsNullOrWhiteSpace(_routingKey) ? _queue : _routingKey;
                channel.QueueBind(_queue, _exchange, rk);
            }
        }
    }

    public Task PublishAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_connection.IsOpen)
            {
                throw new InvalidOperationException("RabbitMQ connection is not open");
            }

            using var channel = _connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(message);
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";

            var rk = string.IsNullOrWhiteSpace(_routingKey) ? _queue : _routingKey;
            if (string.IsNullOrWhiteSpace(rk))
            {
                throw new InvalidOperationException("RabbitMQ routing key or queue is not configured");
            }

            if (!string.IsNullOrWhiteSpace(_exchange))
            {
                channel.BasicPublish(exchange: _exchange, routingKey: rk, basicProperties: props, body: body);
            }
            else
            {
                channel.BasicPublish(exchange: "", routingKey: rk, basicProperties: props, body: body);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // Log error and rethrow so caller can handle it
            System.Diagnostics.Debug.WriteLine($"RabbitMQ publish error: {ex.Message}");
            throw;
        }
    }
}


