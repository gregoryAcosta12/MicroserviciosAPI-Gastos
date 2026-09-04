using API_Usuario_Service.DTOs;

namespace API_Usuario_Service.Services.Interfaces
{
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        Task<List<UsuarioResponseDTO>> GetAllAsync();

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        Task<UsuarioResponseDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene un usuario por email
        /// </summary>
        Task<UsuarioResponseDTO?> GetByEmailAsync(string email);

        /// <summary>
        /// Actualiza un usuario
        /// </summary>
        Task<UsuarioResponseDTO?> UpdateAsync(UsuarioResponseDTO usuarioDTO);

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