using API_Gasto_Service.DTOs.Responses;
using API_Gasto_Service.Repositories.Interfaces;
using API_Gasto_Service.Services.Interfaces;
using Gasto_Service.Repositories.Implementations;

namespace API_Gasto_Service.Services.Implementations
{
    public class ReporteService : IReporteService
    {
        private readonly IGastoRepository _repository;
        private readonly ILogger<ReporteService> _logger;
        private readonly IGastoService _gastoService;

        public ReporteService(IGastoRepository repository, ILogger<ReporteService> logger, IGastoService gastoService)
        {
            _repository = repository;
            _logger = logger;
            _gastoService = gastoService;
        }

        public async Task<ReporteResumenDTO> GetResumenAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var gastos = await _repository.GetByUsuarioIdAsync(usuarioId, fechaInicio, fechaFin);

                if (!gastos.Any())
                {
                    return new ReporteResumenDTO
                    {
                        TotalGastos = 0,
                        PromedioDiario = 0,
                        CantidadGastos = 0,
                        GastoMaximo = 0,
                        GastoMinimo = 0,
                        GastosPorCategoria = new List<CategoriaReporteDTO>(),
                        GastosPorDia = new List<GastoDiarioDTO>()
                    };
                }

                var total = gastos.Sum(g => g.Monto);
                var cantidad = gastos.Count();
                var dias = (fechaFin ?? DateTime.UtcNow).Subtract(fechaInicio ?? DateTime.UtcNow.AddDays(-30)).Days + 1;

                // Gastos por categoría
                var porCategoria = gastos
                    .GroupBy(g => g.CategoriaId)
                    .Select(g => new CategoriaReporteDTO
                    {
                        CategoriaId = g.Key,
                        Total = g.Sum(x => x.Monto),
                        Cantidad = g.Count(),
                        Porcentaje = total > 0 ? (g.Sum(x => x.Monto) / total) * 100 : 0
                    })
                    .ToList();

                // Gastos por día
                var porDia = gastos
                    .GroupBy(g => g.Fecha.Date)
                    .Select(g => new GastoDiarioDTO
                    {
                        Fecha = g.Key,
                        Total = g.Sum(x => x.Monto),
                        Cantidad = g.Count()
                    })
                    .OrderBy(g => g.Fecha)
                    .ToList();

                return new ReporteResumenDTO
                {
                    TotalGastos = total,
                    PromedioDiario = Math.Round(total / dias, 2),
                    CantidadGastos = cantidad,
                    GastoMaximo = gastos.Max(g => g.Monto),
                    GastoMinimo = gastos.Min(g => g.Monto),
                    GastosPorCategoria = porCategoria,
                    GastosPorDia = porDia
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen de gastos");
                throw;
            }
        }

        public async Task<List<CategoriaReporteDTO>> GetReportePorCategoriaAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var gastos = await _repository.GetByUsuarioIdAsync(usuarioId, fechaInicio, fechaFin);
                var total = gastos.Sum(g => g.Monto);

                return gastos
                    .GroupBy(g => g.CategoriaId)
                    .Select(g => new CategoriaReporteDTO
                    {
                        CategoriaId = g.Key,
                        Total = g.Sum(x => x.Monto),
                        Cantidad = g.Count(),
                        Porcentaje = total > 0 ? (g.Sum(x => x.Monto) / total) * 100 : 0
                    })
                    .OrderByDescending(g => g.Total)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte por categoría");
                throw;
            }
        }

        public async Task<ReporteMensualDTO> GetReporteMensualAsync(int usuarioId, int año, int mes)
        {
            try
            {
                var fechaInicio = new DateTime(año, mes, 1);
                var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

                var gastos = await _repository.GetByUsuarioIdAsync(usuarioId, fechaInicio, fechaFin);
                var total = gastos.Sum(g => g.Monto);

                // Gastos por día
                var porDia = gastos
                    .GroupBy(g => g.Fecha.Date)
                    .Select(g => new GastoDiarioDTO
                    {
                        Fecha = g.Key,
                        Total = g.Sum(x => x.Monto),
                        Cantidad = g.Count()
                    })
                    .OrderBy(g => g.Fecha)
                    .ToList();

                // Gastos por categoría
                var porCategoria = gastos
                    .GroupBy(g => g.CategoriaId)
                    .Select(g => new CategoriaReporteDTO
                    {
                        CategoriaId = g.Key,
                        Total = g.Sum(x => x.Monto),
                        Cantidad = g.Count(),
                        Porcentaje = total > 0 ? (g.Sum(x => x.Monto) / total) * 100 : 0
                    })
                    .OrderByDescending(g => g.Total)
                    .ToList();

                return new ReporteMensualDTO
                {
                    Año = año,
                    Mes = mes,
                    Total = total,
                    PromedioDiario = porDia.Any() ? Math.Round(total / porDia.Count, 2) : 0,
                    CantidadGastos = gastos.Count(),
                    GastosPorDia = porDia,
                    GastosPorCategoria = porCategoria
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reporte mensual");
                throw;
            }
        }
    }
}s