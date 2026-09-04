using Microsoft.EntityFrameworkCore;
using API_Categoria_Service.Data;
using API_Categoria_Service.Models;
using API_Categoria_Service.Repositories.Interfaces;

namespace API_Categoria_Service.Repositories.Implementations
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CategoriaRepository> _logger;

        public CategoriaRepository(AppDbContext context, ILogger<CategoriaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Categoria>> GetByUsuarioIdAsync(int usuarioId)
        {
            try
            {
                return await _context.Categorias
                    .Where(c => c.UsuarioId == usuarioId && c.Activo)
                    .OrderBy(c => c.Nombre)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener categorías del usuario {usuarioId}");
                throw;
            }
        }

        public async Task<Categoria?> GetByIdAsync(int id, int usuarioId)
        {
            try
            {
                return await _context.Categorias
                    .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener categoría {id}");
                throw;
            }
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Categorias
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener categoría {id}");
                throw;
            }
        }

        public async Task<Categoria> AddAsync(Categoria categoria)
        {
            try
            {
                await _context.Categorias.AddAsync(categoria);
                await _context.SaveChangesAsync();
                return categoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar categoría");
                throw;
            }
        }

        public async Task<Categoria> UpdateAsync(Categoria categoria)
        {
            try
            {
                _context.Categorias.Update(categoria);
                await _context.SaveChangesAsync();
                return categoria;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar categoría {categoria.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null) return false;

                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar categoría {id}");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.Categorias.AnyAsync(c => c.Id == id && c.Activo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar existencia de categoría {id}");
                return false;
            }
        }

        public async Task<bool> ExistsByNameAsync(string nombre, int usuarioId, int? excludeId = null)
        {
            try
            {
                var query = _context.Categorias
                    .Where(c => c.UsuarioId == usuarioId && c.Nombre.ToLower() == nombre.ToLower());

                if (excludeId.HasValue)
                    query = query.Where(c => c.Id != excludeId.Value);

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar existencia por nombre {nombre}");
                return false;
            }
        }

        public async Task<string?> GetNombreAsync(int id)
        {
            try
            {
                var categoria = await _context.Categorias
                    .Select(c => new { c.Id, c.Nombre })
                    .FirstOrDefaultAsync(c => c.Id == id);

                return categoria?.Nombre;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener nombre de categoría {id}");
                return null;
            }
        }

        public async Task<bool> HasGastosAsync(int id)
        {
            try
            {
                // Esta es una simulación - en realidad consultarías al Gasto-Service
                // Por ahora, asumimos que no tiene gastos si el TotalGastos es 0
                var categoria = await _context.Categorias
                    .Select(c => new { c.Id, c.TotalGastos })
                    .FirstOrDefaultAsync(c => c.Id == id);

                return categoria?.TotalGastos > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar gastos de categoría {id}");
                return true;
            }
        }

        public async Task<bool> IncrementarTotalGastosAsync(int id)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria == null) return false;

                categoria.TotalGastos++;
                categoria.FechaActualizacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al incrementar total gastos de categoría {id}");
                return false;
            }
        }
    }
}