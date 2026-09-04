using API_Gasto_Service.DTOs.Requests;
using API_Gasto_Service.Models;

namespace API_Gasto_Service.Repositories.Interfaces
{
    public interface IGastoRepository
    {
        /// <summary>
        /// Obtiene gastos filtrados
        /// </summary>
        Task<(List<Gasto> Items, int TotalCount)> GetFilteredAsync(GastoFilterDTO filter);

        /// <summary>
        /// Obtiene gastos por usuario
        /// </summary>
        Task<List<Gasto>> GetByUsuarioIdAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null);

        /// <summary>
        /// Obtiene un gasto por ID
        /// </summary>
        Task<Gasto?> GetByIdAsync(int id, int usuarioId);

        /// <summary>
        /// Agrega un nuevo gasto
        /// </summary>
        Task<Gasto> AddAsync(Gasto gasto);

        /// <summary>
        /// Actualiza un gasto
        /// </summary>
        Task<Gasto> UpdateAsync(Gasto gasto);

        /// <summary>
        /// Elimina un gasto
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Obtiene el total de gastos de un usuario
        /// </summary>
        Task<decimal> GetTotalGastosAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null);

        /// <summary>
        /// Obtiene gastos agrupados por categoría
        /// </summary>
        Task<Dictionary<string, decimal>> GetGastosPorCategoriaAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null);
    }
}