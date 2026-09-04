using Microsoft.EntityFrameworkCore;
using API_Usuario_Service.Data;
using API_Usuario_Service.Models;

namespace API_Usuario_Service.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(AppDbContext context, ILogger<UsuarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Usuario>> GetAllAsync()
        {
            try
            {
                return await _context.Usuarios
                    .OrderBy(u => u.Nombre)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw;
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario {id}");
                throw;
            }
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            try
            {
                return await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario por email {email}");
                throw;
            }
        }

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            try
            {
                await _context.Usuarios.AddAsync(usuario);
                await _context.SaveChangesAsync();
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar usuario");
                throw;
            }
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar usuario {usuario.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var usuario = await GetByIdAsync(id);
                if (usuario == null) return false;

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
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
                return await _context.Usuarios
                    .AnyAsync(u => u.Email.ToLower() == email.ToLower());
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
                var usuario = await GetByIdAsync(id);
                if (usuario == null) return false;

                usuario.FechaUltimoLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar último login {id}");
                throw;
            }
        }
    }
}