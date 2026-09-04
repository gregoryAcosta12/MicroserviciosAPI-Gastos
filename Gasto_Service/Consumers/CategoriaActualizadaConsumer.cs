using System;
using Shared.Kernel.Infrastructure.Messaging.Events;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;

namespace API_Gasto_Service.Consumers
{
    public class CategoriaActualizadaConsumer : IEventHandler<CategoriaActualizadaEvent>
    {
        private readonly ILogger<CategoriaActualizadaConsumer> _logger;

        public CategoriaActualizadaConsumer(ILogger<CategoriaActualizadaConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Handle(CategoriaActualizadaEvent @event)
        {
            _logger.LogInformation($"📨 Categoría actualizada: {@event.CategoriaId} - {@event.Nombre}");

            // Aquí podrías actualizar el nombre de la categoría en los gastos
            // o invalidar caché

            await Task.CompletedTask;
        }
    }
}