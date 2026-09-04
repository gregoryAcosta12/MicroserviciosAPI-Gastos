using API_Categoria_Service.DTOs;
using API_Categoria_Service.Models;

namespace API_Categoria_Service.Services.Interfaces
{
    public interface ICategoriaService
    {
        /// <summary>
        /// Obtiene todas las categorías de un usuario
        /// </summary>
        Task<List<CategoriaResponseDTO>> GetByUsuarioIdAsync(int usuarioId);

        /// <summary>
        /// Obtiene una categoría por ID
        /// </summary>
        Task<CategoriaResponseDTO?> GetByIdAsync(int id, int usuarioId);

        /// <summary>
        /// Crea una nueva categoría
        /// </summary>
        Task<CategoriaResponseDTO> CreateAsync(CategoriaCreateDTO dto, int usuarioId);

        /// <summary>
        /// Actualiza una categoría
        /// </summary>
        Task<CategoriaResponseDTO?> UpdateAsync(CategoriaUpdateDTO dto, int usuarioId);

        /// <summary>
        /// Elimina una categoría
        /// </summary>
        Task<bool> DeleteAsync(int id, int usuarioId);

        /// <summary>
        /// Verifica si existe una categoría
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Obtiene el nombre de una categoría
        /// </summary>
        Task<string?> GetNombreAsync(int id);

        /// <summary>
        /// Verifica si una categoría tiene gastos asociados
        /// </summary>
        Task<bool> HasGastosAsync(int id);

        /// <summary>
        /// Incrementa el contador de gastos de una categoría
        /// </summary>
        Task<bool> IncrementarTotalGastosAsync(int id);
    }
}