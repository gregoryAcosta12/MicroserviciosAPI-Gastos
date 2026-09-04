using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Kernel.Infrastructure.Events;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;

namespace Shared.Kernel.Infrastructure.Messaging.Implementations
{
    public class RabbitMQMessageBus : IMessageBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQMessageBus> _logger;
        private readonly Dictionary<string, List<Type>> _handlers = new();
        private bool _disposed;

        public bool IsConnected => _connection?.IsOpen ?? false;

        public RabbitMQMessageBus(
            string hostName = "localhost",
            string userName = "guest",
            string password = "guest",
            int port = 5672,
            ILogger<RabbitMQMessageBus>? logger = null)
        {
            _logger = logger ?? new LoggerFactory().CreateLogger<RabbitMQMessageBus>();

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = hostName,
                    UserName = userName,
                    Password = password,
                    Port = port,
                    VirtualHost = "/",
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declarar exchanges principales
                DeclareExchanges();

                _logger.LogInformation("✅ Conexión a RabbitMQ establecida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al conectar con RabbitMQ");
                throw;
            }
        }

        private void DeclareExchanges()
        {
            _channel.ExchangeDeclare("gastos-exchange", ExchangeType.Topic, durable: true);
            _channel.ExchangeDeclare("categorias-exchange", ExchangeType.Topic, durable: true);
            _channel.ExchangeDeclare("usuarios-exchange", ExchangeType.Topic, durable: true);
        }

        public async Task Publish<T>(T @event) where T : IntegrationEvent
        {
            if (!IsConnected)
            {
                _logger.LogWarning("⚠️ No se puede publicar: RabbitMQ no está conectado");
                return;
            }

            try
            {
                var eventName = @event.GetType().Name;
                var exchangeName = GetExchangeName(eventName);
                var routingKey = GetRoutingKey(eventName);

                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Type = eventName;
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body
                );

                _logger.LogInformation($"📤 Evento publicado: {eventName} | Exchange: {exchangeName} | RoutingKey: {routingKey}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al publicar evento: {typeof(T).Name}");
                throw;
            }
        }

        public async Task Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IEventHandler<T>
        {
            if (!IsConnected)
            {
                _logger.LogWarning("⚠️ No se puede suscribir: RabbitMQ no está conectado");
                return;
            }

            try
            {
                var eventName = typeof(T).Name;
                var queueName = GetQueueName(eventName);
                var exchangeName = GetExchangeName(eventName);
                var routingKey = GetRoutingKey(eventName);

                // Registrar handler
                var handlerType = typeof(TH);
                if (!_handlers.ContainsKey(eventName))
                {
                    _handlers[eventName] = new List<Type>();
                }
                _handlers[eventName].Add(handlerType);

                // Declarar cola
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                _channel.QueueBind(
                    queue: queueName,
                    exchange: exchangeName,
                    routingKey: routingKey
                );

                // Configurar consumidor
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var @event = JsonSerializer.Deserialize<T>(message);

                        if (@event != null && _handlers.TryGetValue(eventName, out var handlerTypes))
                        {
                            foreach (var handlerType in handlerTypes)
                            {
                                var handler = Activator.CreateInstance(handlerType) as IEventHandler<T>;
                                if (handler != null)
                                {
                                    await handler.Handle(@event);
                                    _logger.LogInformation($"📨 Evento manejado: {eventName} | Handler: {handlerType.Name}");
                                }
                            }
                        }

                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"❌ Error al procesar evento: {eventName}");
                        _channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                };

                _channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer
                );

                _logger.LogInformation($"📥 Suscripción creada: {eventName} | Queue: {queueName} | Handler: {typeof(TH).Name}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al suscribir: {typeof(T).Name} -> {typeof(TH).Name}");
                throw;
            }
        }

        public async Task Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IEventHandler<T>
        {
            try
            {
                var eventName = typeof(T).Name;
                var handlerType = typeof(TH);

                if (_handlers.TryGetValue(eventName, out var handlerTypes))
                {
                    handlerTypes.Remove(handlerType);
                    if (!handlerTypes.Any())
                    {
                        _handlers.Remove(eventName);
                    }
                }

                _logger.LogInformation($"🔕 Desuscripción: {eventName} | Handler: {handlerType.Name}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al desuscribir: {typeof(T).Name} -> {typeof(TH).Name}");
                throw;
            }
        }

        private string GetExchangeName(string eventName)
        {
            if (eventName.Contains("Gasto")) return "gastos-exchange";
            if (eventName.Contains("Categoria")) return "categorias-exchange";
            if (eventName.Contains("Usuario")) return "usuarios-exchange";
            return "default-exchange";
        }

        private string GetRoutingKey(string eventName)
        {
            return eventName.ToLower();
        }

        private string GetQueueName(string eventName)
        {
            return $"{eventName.ToLower()}-queue";
        }

        public void Dispose()
        {
            if (_disposed) return;

            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();

            _disposed = true;
            _logger.LogInformation("🔌 Conexión a RabbitMQ cerrada");
        }
    }
}