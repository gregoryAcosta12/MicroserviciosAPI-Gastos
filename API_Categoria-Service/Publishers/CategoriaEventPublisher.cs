using API_Categoria_Service.Models;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;
using Shared.Kernel.Infrastructure.Messaging.Events;

namespace API_Categoria_Service.Publishers
{
    public class CategoriaEventPublisher
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger<CategoriaEventPublisher> _logger;

        public CategoriaEventPublisher(IMessageBus messageBus, ILogger<CategoriaEventPublisher> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }

        public async Task PublishCategoriaCreadaAsync(Categoria categoria)
        {
            try
            {
                var evento = new CategoriaCreadaEvent
                {
                    CategoriaId = categoria.Id,
                    Nombre = categoria.Nombre,
                    UsuarioId = categoria.UsuarioId
                };

                await _messageBus.Publish(evento);
                _logger.LogInformation($"Evento CategoriaCreada publicado: {categoria.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar evento CategoriaCreada: {categoria.Id}");
            }
        }

        public async Task PublishCategoriaActualizadaAsync(Categoria categoria)
        {
            try
            {
                var evento = new CategoriaActualizadaEvent
                {
                    CategoriaId = categoria.Id,
                    Nombre = categoria.Nombre,
                    UsuarioId = categoria.UsuarioId,
                    Descripcion = categoria.Descripcion
                };

                await _messageBus.Publish(evento);
                _logger.LogInformation($"Evento CategoriaActualizada publicado: {categoria.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar evento CategoriaActualizada: {categoria.Id}");
            }
        }

        public async Task PublishCategoriaEliminadaAsync(int categoriaId)
        {
            try
            {
                var evento = new CategoriaEliminadaEvent
                {
                    CategoriaId = categoriaId
                };

                await _messageBus.Publish(evento);
                _logger.LogInformation($"Evento CategoriaEliminada publicado: {categoriaId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al publicar evento CategoriaEliminada: {categoriaId}");
            }
        }
    }
}