using API_Gasto_Service.Models;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;
using Shared.Kernel.Infrastructure.Messaging.Events;

namespace API_Gasto_Service.Publishers
{
    public class GastoEventPublisher
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<GastoEventPublisher> _logger;

        public GastoEventPublisher(IMessageBus messageBus, ILogger<GastoEventPublisher> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }

        public async Task PublishGastoCreadoAsync(Gasto gasto)
        {
            try
            {
                var evento = new GastoCreadoEvent
                {
                    GastoId = gasto.Id,
                    Monto = gasto.Monto,
                    CategoriaId = gasto.CategoriaId,
                    UsuarioId = gasto.UsuarioId,
                    Fecha = gasto.Fecha,
                    Descripcion = gasto.Descripcion
                };

                await _messageBus.Publish(evento);
                _logger.LogInformation($"Evento GastoCreado publicado: {gasto.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar evento GastoCreado: {gasto.Id}");
            }
        }

        public async Task PublishGastoActualizadoAsync(Gasto gasto)
        {
            try
            {
                var evento = new GastoActualizadoEvent
                {
                    GastoId = gasto.Id,
                    Monto = gasto.Monto,
                    CategoriaId = gasto.CategoriaId,
                    UsuarioId = gasto.UsuarioId,
                    Fecha = gasto.Fecha
                };

                await _messageBus.Publish(evento);
                _logger.LogInformation($"Evento GastoActualizado publicado: {gasto.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar evento GastoActualizado: {gasto.Id}");
            }
        }

        public async Task PublishGastoEliminadoAsync(int gastoId)
        {
            try
            {
                var evento = new GastoEliminadoEvent
                {
                    GastoId = gastoId
                };

                await _messageBus.Publish(evento);
                _logger.LogInformation($"Evento GastoEliminado publicado: {gastoId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar evento GastoEliminado: {gastoId}");
            }
        }
    }
}