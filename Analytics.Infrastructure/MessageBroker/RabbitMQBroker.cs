using Analytics.Core.Domain.Contracts.Infrastructure;
using Analytics.Shared.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Analytics.Infrastructure.MessageBroker;

public class RabbitMQBroker : IMessageBroker, IDisposable
{
    private readonly RabbitMQSettings _settings;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly object _lock = new object();

    public RabbitMQBroker(
        IOptions<RabbitMQSettings> settings
        )
    {
        _settings = settings.Value;
        InitializeConnectionWithRetry();
    }

    private void InitializeConnectionWithRetry(int maxRetries = 5)
    {
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                attempt++;

                var factory = new ConnectionFactory
                {
                    HostName = _settings.Host,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedConnectionTimeout = TimeSpan.FromSeconds(30)
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();


                // Setup queues and exchanges
                SetupRabbitMQInfrastructure();

                return; // Success!
            }
            catch (Exception ex)
            {

                if (attempt < maxRetries)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // Exponential backoff
                    Thread.Sleep(delay);
                }
                else
                {
                    // Don't throw - allow app to start without RabbitMQ for now
                }
            }
        }
    }

    private void SetupRabbitMQInfrastructure()
    {
        if (_channel is null)
        {
            throw new InvalidOperationException("RabbitMQ channel is not initialized");
        }

        // Declare exchange (Fanout type)
        _channel.ExchangeDeclare(
            exchange: _settings.Exchange,
            type: ExchangeType.Fanout,
            durable: true, // restart يعني يفضل موجود حتى بعد 
            autoDelete: false 
        );

        // Declare main queue with DLQ
        var queueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", "" },
            { "x-dead-letter-routing-key", _settings.DeadLetterQueue }
        };

        _channel.QueueDeclare(
            queue: _settings.Queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArgs
        );

        // Bind queue to exchange
        _channel.QueueBind(
            queue: _settings.Queue,
            exchange: _settings.Exchange,
            routingKey: ""
        );

        // Declare Dead Letter Queue
        _channel.QueueDeclare(
            queue: _settings.DeadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
    }

    private void EnsureConnectionIsOpen()
    {
        lock (_lock)
        {
            if (_connection == null || !_connection.IsOpen || _channel == null || !_channel.IsOpen)
            {
                InitializeConnectionWithRetry();
            }
        }
    }

    public Task PublishAsync<T>( T message)
    {
        try
        {
            EnsureConnectionIsOpen();

            if (_channel is null)
            {
                throw new InvalidOperationException("RabbitMQ is not available");
            }

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.MessageId = Guid.NewGuid().ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            // every message go to exchange then it distributes its on Queue
            _channel.BasicPublish(
                exchange: _settings.Exchange,
                routingKey: "",
                basicProperties: properties,
                body: body
            );

                

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to publish message to RabbitMQ", ex);

        }
    }

    public void StartConsuming(string queueName, Func<string, Task<bool>> messageHandler)
    {
        try
        {
            EnsureConnectionIsOpen();

            if (_channel is null)
            {
                return;
            }

            var consumer = new EventingBasicConsumer(_channel);

            // Set prefetch count
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var deliveryTag = ea.DeliveryTag;
                var messageId = ea.BasicProperties.MessageId ?? "Unknown";


                try
                {
                    var success = await messageHandler(message);

                    if (success && _channel != null && _channel.IsOpen)
                    {
                        _channel.BasicAck(deliveryTag, multiple: false);
                    }
                    else if (_channel != null && _channel.IsOpen)
                    {
                        _channel.BasicNack(deliveryTag, multiple: false, requeue: false);
                    }
                }
                catch (Exception ex)
                {
                    if (_channel != null && _channel.IsOpen)
                    {
                        _channel.BasicNack(deliveryTag, multiple: false, requeue: false);
                    }
                }
            };

            _channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}