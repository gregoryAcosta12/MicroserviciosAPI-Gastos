using API_Categoria_Service.Models;

namespace API_Categoria_Service.Repositories.Interfaces
{
    public interface ICategoriaRepository
    {
        /// <summary>
        /// Obtiene todas las categorías de un usuario
        /// </summary>
        Task<List<Categoria>> GetByUsuarioIdAsync(int usuarioId);

        /// <summary>
        /// Obtiene una categoría por ID
        /// </summary>
        Task<Categoria?> GetByIdAsync(int id, int usuarioId);

        /// <summary>
        /// Obtiene una categoría por ID sin verificar usuario
        /// </summary>
        Task<Categoria?> GetByIdAsync(int id);

        /// <summary>
        /// Agrega una nueva categoría
        /// </summary>
        Task<Categoria> AddAsync(Categoria categoria);

        /// <summary>
        /// Actualiza una categoría
        /// </summary>
        Task<Categoria> UpdateAsync(Categoria categoria);

        /// <summary>
        /// Elimina una categoría
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica si existe una categoría por ID
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Verifica si existe una categoría por nombre
        /// </summary>
        Task<bool> ExistsByNameAsync(string nombre, int usuarioId, int? excludeId = null);

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