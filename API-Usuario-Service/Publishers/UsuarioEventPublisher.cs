using System.Text.Json;
using RabbitMQ.Client;
using API_Usuario_Service.Models;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;

namespace API_Usuario_Service.Publishers
{
    public class UsuarioEventPublisher
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<UsuarioEventPublisher> _logger;

        public UsuarioEventPublisher(IMessageBus messageBus, ILogger<UsuarioEventPublisher> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }

        public async Task PublishUsuarioCreadoAsync(Usuario usuario)
        {
            try
            {
                var evento = new UsuarioCreadoEvent
                {
                    UsuarioId = usuario.Id,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    FechaCreacion = DateTime.UtcNow
                };

                await _messageBus.Publish(evento);
                _logger.LogInformation($"Evento UsuarioCreado publicado: {usuario.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar evento UsuarioCreado: {usuario.Id}");
                throw;
            }
        }
    }

    public class UsuarioCreadoEvent : IntegrationEvent
    {
        public int UsuarioId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }

    public class IntegrationEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }

    public interface IMessageBus
    {
        Task Publish<T>(T @event) where T : IntegrationEvent;
    }
}