using API_Usuario_Service.DTOs;
using API_Usuario_Service.Models;
using API_Usuario_Service.Repositories;
using API_Usuario_Service.Services.Interfaces;

namespace API_Usuario_Service.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<List<UsuarioResponseDTO>> GetAllAsync()
        {
            try
            {
                var usuarios = await _usuarioRepository.GetAllAsync();
                return usuarios.Select(u => new UsuarioResponseDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    Rol = u.Rol,
                    Activo = u.Activo,
                    FechaCreacion = u.FechaCreacion,
                    FechaUltimoLogin = u.FechaUltimoLogin
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw;
            }
        }

        public async Task<UsuarioResponseDTO?> GetByIdAsync(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null) return null;

                return new UsuarioResponseDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    Activo = usuario.Activo,
                    FechaCreacion = usuario.FechaCreacion,
                    FechaUltimoLogin = usuario.FechaUltimoLogin
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario {id}");
                throw;
            }
        }

        public async Task<UsuarioResponseDTO?> GetByEmailAsync(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByEmailAsync(email);
                if (usuario == null) return null;

                return new UsuarioResponseDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    Activo = usuario.Activo,
                    FechaCreacion = usuario.FechaCreacion,
                    FechaUltimoLogin = usuario.FechaUltimoLogin
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario por email {email}");
                throw;
            }
        }

        public async Task<UsuarioResponseDTO?> UpdateAsync(UsuarioResponseDTO usuarioDTO)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(usuarioDTO.Id);
                if (usuario == null) return null;

                usuario.Nombre = usuarioDTO.Nombre;
                usuario.Email = usuarioDTO.Email;
                usuario.Rol = usuarioDTO.Rol;
                usuario.Activo = usuarioDTO.Activo;
                usuario.FechaActualizacion = DateTime.UtcNow;

                await _usuarioRepository.UpdateAsync(usuario);

                return new UsuarioResponseDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    Activo = usuario.Activo,
                    FechaCreacion = usuario.FechaCreacion,
                    FechaUltimoLogin = usuario.FechaUltimoLogin
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar usuario {usuarioDTO.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByIdAsync(id);
                if (usuario == null) return false;

                await _usuarioRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar usuario {id}");
                throw;
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            try
            {
                return await _usuarioRepository.ExistsByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar email {email}");
                throw;
            }
        }

        public async Task<bool> UpdateLastLoginAsync(int id)
        {
            try
            {
                return await _usuarioRepository.UpdateLastLoginAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar último login {id}");
                throw;
            }
        }
    }
}