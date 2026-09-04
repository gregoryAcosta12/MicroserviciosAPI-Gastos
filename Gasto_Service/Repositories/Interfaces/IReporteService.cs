using API_Gasto_Service.DTOs.Responses;

namespace API_Gasto_Service.Services.Interfaces
{
    public interface IReporteService
    {
        /// <summary>
        /// Obtiene el resumen de gastos
        /// </summary>
        Task<ReporteResumenDTO> GetResumenAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null);

        /// <summary>
        /// Obtiene el reporte por categorías
        /// </summary>
        Task<List<CategoriaReporteDTO>> GetReportePorCategoriaAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null);

        /// <summary>
        /// Obtiene el reporte mensual
        /// </summary>
        Task<ReporteMensualDTO> GetReporteMensualAsync(int usuarioId, int año, int mes);
    }

    public class ReporteResumenDTO
    {
        public decimal TotalGastos { get; set; }
        public decimal PromedioDiario { get; set; }
        public int CantidadGastos { get; set; }
        public decimal GastoMaximo { get; set; }
        public decimal GastoMinimo { get; set; }
        public List<CategoriaReporteDTO> GastosPorCategoria { get; set; } = new();
        public List<GastoDiarioDTO> GastosPorDia { get; set; } = new();
    }

    public class CategoriaReporteDTO
    {
        public int CategoriaId { get; set; }
        public string? NombreCategoria { get; set; }
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
        public decimal Porcentaje { get; set; }
    }

    public class GastoDiarioDTO
    {
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
    }

    public class ReporteMensualDTO
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public decimal Total { get; set; }
        public decimal PromedioDiario { get; set; }
        public int CantidadGastos { get; set; }
        public List<GastoDiarioDTO> GastosPorDia { get; set; } = new();
        public List<CategoriaReporteDTO> GastosPorCategoria { get; set; } = new();
    }
}