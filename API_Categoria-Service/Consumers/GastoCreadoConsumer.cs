using System;
using API_Categoria_Service.Services.Interfaces;
using Shared.Kernel.Infrastructure.Messaging.Events;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;

namespace API_Categoria_Service.Consumers
{
    public class GastoCreadoConsumer : IEventHandler<GastoCreadoEvent>
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<GastoCreadoConsumer> _logger;

        public GastoCreadoConsumer(ICategoriaService categoriaService, ILogger<GastoCreadoConsumer> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        public async Task Handle(GastoCreadoEvent @event)
        {
            try
            {
                _logger.LogInformation($"📨 Gasto creado: {@event.GastoId} - Categoría: {@event.CategoriaId} - Monto: {@event.Monto}");

                // Incrementar el contador de gastos de la categoría
                var result = await _categoriaService.IncrementarTotalGastosAsync(@event.CategoriaId);

                if (result)
                {
                    _logger.LogInformation($"✅ Total de gastos incrementado para categoría {@event.CategoriaId}");
                }
                else
                {
                    _logger.LogWarning($"⚠️ No se pudo incrementar total de gastos para categoría {@event.CategoriaId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al procesar evento GastoCreado: {@event.GastoId}");
            }
        }
    }
}