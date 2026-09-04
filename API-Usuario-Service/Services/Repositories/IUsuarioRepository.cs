using API_Usuario_Service.Models;

namespace API_Usuario_Service.Repositories
{
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        Task<List<Usuario>> GetAllAsync();

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        Task<Usuario?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un usuario por email
        /// </summary>
        Task<Usuario?> GetByEmailAsync(string email);

        /// <summary>
        /// Agrega un nuevo usuario
        /// </summary>
        Task<Usuario> AddAsync(Usuario usuario);

        /// <summary>
        /// Actualiza un usuario
        /// </summary>
        Task<Usuario> UpdateAsync(Usuario usuario);

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica si existe un usuario por email
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email);

        /// <summary>
        /// Actualiza la fecha de último login
        /// </summary>
        Task<bool> UpdateLastLoginAsync(int id);
    }
}