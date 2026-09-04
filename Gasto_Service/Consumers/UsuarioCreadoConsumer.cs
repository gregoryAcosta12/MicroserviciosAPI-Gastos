using System;
using Shared.Kernel.Infrastructure.Messaging.Events;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;

namespace API_Gasto_Service.Consumers
{
    public class UsuarioCreadoConsumer : IEventHandler<UsuarioCreadoEvent>
    {
        private readonly ILogger<UsuarioCreadoConsumer> _logger;

        public UsuarioCreadoConsumer(ILogger<UsuarioCreadoConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Handle(UsuarioCreadoEvent @event)
        {
            _logger.LogInformation($"📨 Usuario creado: {@event.UsuarioId} - {@event.Email}");

            // Aquí podrías crear configuración inicial para el nuevo usuario
            // Por ejemplo: crear categorías por defecto

            await Task.CompletedTask;
        }
    }
}