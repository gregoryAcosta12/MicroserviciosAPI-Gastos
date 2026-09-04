using API_Usuario_Service.DTOs;

namespace API_Usuario_Service.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Inicia sesión de usuario
        /// </summary>
        Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDTO);

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        Task<UsuarioResponseDTO?> RegisterAsync(RegisterDTO registerDTO);

        /// <summary>
        /// Genera un token JWT
        /// </summary>
        string GenerateJwtToken(Models.Usuario usuario);

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        bool ValidateToken(string token);
    }
}