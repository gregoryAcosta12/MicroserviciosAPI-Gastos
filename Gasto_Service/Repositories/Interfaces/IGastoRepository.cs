using API_Gasto_Service.DTOs.Requests;
using API_Gasto_Service.DTOs.Responses;

namespace API_Gasto_Service.Services.Interfaces
{
    public interface IGastoService
    {
        /// <summary>
        /// Obtiene gastos filtrados
        /// </summary>
        Task<PaginatedResponseDTO<GastoResponseDTO>> GetFilteredAsync(GastoFilterDTO filter);

        /// <summary>
        /// Obtiene un gasto por ID
        /// </summary>
        Task<GastoDetailDTO?> GetByIdAsync(int id, int usuarioId);

        /// <summary>
        /// Crea un nuevo gasto
        /// </summary>
        Task<GastoResponseDTO> CreateAsync(GastoCreateDTO dto, int usuarioId);

        /// <summary>
        /// Actualiza un gasto
        /// </summary>
        Task<GastoResponseDTO?> UpdateAsync(GastoUpdateDTO dto, int usuarioId);

        /// <summary>
        /// Elimina un gasto
        /// </summary>
        Task<bool> DeleteAsync(int id, int usuarioId);

        /// <summary>
        /// Obtiene el total de gastos de un usuario
        /// </summary>
        Task<decimal> GetTotalGastosAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null);

        /// <summary>
        /// Obtiene gastos por categoría
        /// </summary>
        Task<Dictionary<string, decimal>> GetGastosPorCategoriaAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null);
    }

    public class PaginatedResponseDTO<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new();
    }
}