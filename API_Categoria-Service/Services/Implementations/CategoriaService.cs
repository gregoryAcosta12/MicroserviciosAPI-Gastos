using AutoMapper;
using API_Categoria_Service.DTOs;
using API_Categoria_Service.Models;
using API_Categoria_Service.Publishers;
using API_Categoria_Service.Repositories.Interfaces;
using API_Categoria_Service.Services.Interfaces;

namespace API_Categoria_Service.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoriaService> _logger;
        private readonly CategoriaEventPublisher _eventPublisher;

        public CategoriaService(
            ICategoriaRepository repository,
            IMapper mapper,
            ILogger<CategoriaService> logger,
            CategoriaEventPublisher eventPublisher)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<List<CategoriaResponseDTO>> GetByUsuarioIdAsync(int usuarioId)
        {
            try
            {
                var categorias = await _repository.GetByUsuarioIdAsync(usuarioId);
                return _mapper.Map<List<CategoriaResponseDTO>>(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener categorías del usuario {usuarioId}");
                throw;
            }
        }

        public async Task<CategoriaResponseDTO?> GetByIdAsync(int id, int usuarioId)
        {
            try
            {
                var categoria = await _repository.GetByIdAsync(id, usuarioId);
                if (categoria == null) return null;

                return _mapper.Map<CategoriaResponseDTO>(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener categoría {id}");
                throw;
            }
        }

        public async Task<CategoriaResponseDTO> CreateAsync(CategoriaCreateDTO dto, int usuarioId)
        {
            try
            {
                // Verificar si ya existe una categoría con el mismo nombre
                var existe = await _repository.ExistsByNameAsync(dto.Nombre, usuarioId);
                if (existe)
                    throw new ArgumentException($"Ya existe una categoría con el nombre '{dto.Nombre}'");

                var categoria = _mapper.Map<Categoria>(dto);
                categoria.UsuarioId = usuarioId;
                categoria.FechaCreacion = DateTime.UtcNow;

                await _repository.AddAsync(categoria);

                // Publicar evento
                await _eventPublisher.PublishCategoriaCreadaAsync(categoria);

                return _mapper.Map<CategoriaResponseDTO>(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                throw;
            }
        }

        public async Task<CategoriaResponseDTO?> UpdateAsync(CategoriaUpdateDTO dto, int usuarioId)
        {
            try
            {
                var categoria = await _repository.GetByIdAsync(dto.Id, usuarioId);
                if (categoria == null) return null;

                // Verificar si ya existe otra categoría con el mismo nombre
                var existe = await _repository.ExistsByNameAsync(dto.Nombre, usuarioId, dto.Id);
                if (existe)
                    throw new ArgumentException($"Ya existe otra categoría con el nombre '{dto.Nombre}'");

                _mapper.Map(dto, categoria);
                categoria.FechaActualizacion = DateTime.UtcNow;

                await _repository.UpdateAsync(categoria);

                // Publicar evento
                await _eventPublisher.PublishCategoriaActualizadaAsync(categoria);

                return _mapper.Map<CategoriaResponseDTO>(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar categoría {dto.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId)
        {
            try
            {
                var categoria = await _repository.GetByIdAsync(id, usuarioId);
                if (categoria == null) return false;

                // Verificar si tiene gastos asociados
                var hasGastos = await HasGastosAsync(id);
                if (hasGastos)
                    throw new InvalidOperationException("No se puede eliminar la categoría porque tiene gastos asociados");

                await _repository.DeleteAsync(id);

                // Publicar evento
                await _eventPublisher.PublishCategoriaEliminadaAsync(id);

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
                return await _repository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar existencia de categoría {id}");
                return false;
            }
        }

        public async Task<string?> GetNombreAsync(int id)
        {
            try
            {
                return await _repository.GetNombreAsync(id);
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
                return await _repository.HasGastosAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar gastos de categoría {id}");
                return true; // Por seguridad, asumimos que tiene gastos
            }
        }

        public async Task<bool> IncrementarTotalGastosAsync(int id)
        {
            try
            {
                return await _repository.IncrementarTotalGastosAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al incrementar total gastos de categoría {id}");
                return false;
            }
        }
    }
}