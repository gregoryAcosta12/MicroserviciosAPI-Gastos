using Microsoft.EntityFrameworkCore;
using API_Gasto_Service.Data;
using API_Gasto_Service.DTOs.Requests;
using API_Gasto_Service.Models;
using API_Gasto_Service.Repositories.Interfaces;

namespace API_Gasto_Service.Repositories.Implementations
{
    public class GastoRepository : IGastoRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GastoRepository> _logger;

        public GastoRepository(AppDbContext context, ILogger<GastoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(List<Gasto> Items, int TotalCount)> GetFilteredAsync(GastoFilterDTO filter)
        {
            try
            {
                var query = _context.Gastos
                    .Include(g => g.Detalles)
                    .AsQueryable();

                // Aplicar filtros
                if (filter.UsuarioId.HasValue)
                    query = query.Where(g => g.UsuarioId == filter.UsuarioId.Value);

                if (filter.CategoriaId.HasValue)
                    query = query.Where(g => g.CategoriaId == filter.CategoriaId.Value);

                if (filter.FechaInicio.HasValue)
                    query = query.Where(g => g.Fecha >= filter.FechaInicio.Value);

                if (filter.FechaFin.HasValue)
                    query = query.Where(g => g.Fecha <= filter.FechaFin.Value);

                if (filter.MontoMinimo.HasValue)
                    query = query.Where(g => g.Monto >= filter.MontoMinimo.Value);

                if (filter.MontoMaximo.HasValue)
                    query = query.Where(g => g.Monto <= filter.MontoMaximo.Value);

                if (!string.IsNullOrEmpty(filter.Estado))
                    query = query.Where(g => g.Estado == filter.Estado);

                // Ordenar
                query = filter.SortBy?.ToLower() switch
                {
                    "monto" => filter.SortDescending ? query.OrderByDescending(g => g.Monto) : query.OrderBy(g => g.Monto),
                    "fecha" => filter.SortDescending ? query.OrderByDescending(g => g.Fecha) : query.OrderBy(g => g.Fecha),
                    "descripcion" => filter.SortDescending ? query.OrderByDescending(g => g.Descripcion) : query.OrderBy(g => g.Descripcion),
                    _ => filter.SortDescending ? query.OrderByDescending(g => g.Fecha) : query.OrderBy(g => g.Fecha)
                };

                var totalCount = await query.CountAsync();

                // Paginación
                var items = await query
                    .Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize)
                    .ToListAsync();

                return (items, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener gastos filtrados");
                throw;
            }
        }

        public async Task<List<Gasto>> GetByUsuarioIdAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Gastos
                    .Include(g => g.Detalles)
                    .Where(g => g.UsuarioId == usuarioId);

                if (fechaInicio.HasValue)
                    query = query.Where(g => g.Fecha >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(g => g.Fecha <= fechaFin.Value);

                return await query.OrderByDescending(g => g.Fecha).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener gastos del usuario {usuarioId}");
                throw;
            }
        }

        public async Task<Gasto?> GetByIdAsync(int id, int usuarioId)
        {
            try
            {
                return await _context.Gastos
                    .Include(g => g.Detalles)
                    .FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener gasto {id}");
                throw;
            }
        }

        public async Task<Gasto> AddAsync(Gasto gasto)
        {
            try
            {
                await _context.Gastos.AddAsync(gasto);
                await _context.SaveChangesAsync();
                return gasto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar gasto");
                throw;
            }
        }

        public async Task<Gasto> UpdateAsync(Gasto gasto)
        {
            try
            {
                _context.Gastos.Update(gasto);
                await _context.SaveChangesAsync();
                return gasto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar gasto {gasto.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var gasto = await _context.Gastos.FindAsync(id);
                if (gasto == null) return false;

                _context.Gastos.Remove(gasto);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar gasto {id}");
                throw;
            }
        }

        public async Task<decimal> GetTotalGastosAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Gastos.Where(g => g.UsuarioId == usuarioId);

                if (fechaInicio.HasValue)
                    query = query.Where(g => g.Fecha >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(g => g.Fecha <= fechaFin.Value);

                return await query.SumAsync(g => g.Monto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener total de gastos");
                throw;
            }
        }

        public async Task<Dictionary<string, decimal>> GetGastosPorCategoriaAsync(int usuarioId, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Gastos.Where(g => g.UsuarioId == usuarioId);

                if (fechaInicio.HasValue)
                    query = query.Where(g => g.Fecha >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(g => g.Fecha <= fechaFin.Value);

                return await query
                    .GroupBy(g => g.CategoriaId.ToString())
                    .Select(g => new { CategoriaId = g.Key, Total = g.Sum(x => x.Monto) })
                    .ToDictionaryAsync(k => k.CategoriaId, v => v.Total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener gastos por categoría");
                throw;
            }
        }
    }
}