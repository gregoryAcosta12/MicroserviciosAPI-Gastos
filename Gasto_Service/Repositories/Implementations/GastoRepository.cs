using System.Net.Http.Headers;
using System.Text.Json;
using API_Gasto_Service.DTOs.Requests;
using API_Gasto_Service.DTOs.Responses;
using API_Gasto_Service.Models;
using API_Gasto_Service.Publishers;
using API_Gasto_Service.Repositories.Interfaces;
using API_Gasto_Service.Services.Interfaces;
using AutoMapper;
using Gasto_Service.Publishers;
using Gasto_Service.Repositories.Implementations;

namespace API_Gasto_Service.Services.Implementations
{
    public class GastoService : IGastoService
    {
        private readonly IGastoRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GastoService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly GastoEventPublisher _eventPublisher;

        public GastoService(
            IGastoRepository repository,
            IMapper mapper,
            ILogger<GastoService> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            GastoEventPublisher eventPublisher)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _eventPublisher = eventPublisher;
        }

        public async Task<PaginatedResponseDTO<GastoResponseDTO>> GetFilteredAsync(GastoFilterDTO filter)
        {
            try
            {
                var (gastos, totalCount) = await _repository.GetFilteredAsync(filter);

                var items = _mapper.Map<List<GastoResponseDTO>>(gastos);

                // Enriquecer con nombre de categoría (si es necesario)
                foreach (var item in items)
                {
                    item.NombreCategoria = await GetCategoriaNombreAsync(item.CategoriaId);
                }

                return new PaginatedResponseDTO<GastoResponseDTO>
                {
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                    Items = items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener gastos filtrados");
                throw;
            }
        }

        public async Task<GastoDetailDTO?> GetByIdAsync(int id, int usuarioId)
        {
            try
            {
                var gasto = await _repository.GetByIdAsync(id, usuarioId);
                if (gasto == null) return null;

                var dto = _mapper.Map<GastoDetailDTO>(gasto);
                dto.NombreCategoria = await GetCategoriaNombreAsync(gasto.CategoriaId);
                dto.TotalDetalles = gasto.Detalles?.Count ?? 0;

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener gasto {id}");
                throw;
            }
        }

        public async Task<GastoResponseDTO> CreateAsync(GastoCreateDTO dto, int usuarioId)
        {
            try
            {
                // Validar categoría
                var categoriaValida = await ValidarCategoriaAsync(dto.CategoriaId);
                if (!categoriaValida)
                    throw new ArgumentException($"La categoría {dto.CategoriaId} no existe");

                // Crear gasto
                var gasto = _mapper.Map<Gasto>(dto);
                gasto.UsuarioId = usuarioId;
                gasto.FechaCreacion = DateTime.UtcNow;

                await _repository.AddAsync(gasto);

                // Publicar evento
                await _eventPublisher.PublishGastoCreadoAsync(gasto);

                var response = _mapper.Map<GastoResponseDTO>(gasto);
                response.NombreCategoria = await GetCategoriaNombreAsync(gasto.CategoriaId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear gasto");
                throw;
            }
        }

        public async Task<GastoResponseDTO?> UpdateAsync(GastoUpdateDTO dto, int usuarioId)
        {
            try
            {
                var gasto = await _repository.GetByIdAsync(dto.Id, usuarioId);
                if (gasto == null) return null;

                // Validar nueva categoría
                var categoriaValida = await ValidarCategoriaAsync(dto.CategoriaId);
                if (!categoriaValida)
                    throw new ArgumentException($"La categoría {dto.CategoriaId} no existe");

                // Actualizar
                _mapper.Map(dto, gasto);
                gasto.FechaActualizacion = DateTime.UtcNow;

                await _repository.UpdateAsync(gasto);

                // Publicar evento
                await _eventPublisher.PublishGastoActualizadoAsync(gasto);

                var response = _mapper.Map<GastoResponseDTO>(gasto);
                response.NombreCategoria = await GetCategoriaNombreAsync(gasto.CategoriaId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar gasto {dto.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId)
        {
            try
            {
                var gasto = await _repository.GetByIdAsync(id, usuarioId);
                if (gasto == null) return false;

                await _repository.DeleteAsync(id);

                // Publicar evento
                await _eventPublisher.PublishGastoEliminadoAsync(id);

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
                return await _repository.GetTotalGastosAsync(usuarioId, fechaInicio, fechaFin);
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
                return await _repository.GetGastosPorCategoriaAsync(usuarioId, fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener gastos por categoría");
                throw;
            }
        }

        private async Task<bool> ValidarCategoriaAsync(int categoriaId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_configuration["ServiceUrls:CategoriaService"]}/api/categorias/{categoriaId}/validar");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<Dictionary<string, bool>>(content);
                    return result != null && result.GetValueOrDefault("existe", false);
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error al validar categoría {categoriaId}");
                return false;
            }
        }

        private async Task<string?> GetCategoriaNombreAsync(int categoriaId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"{_configuration["ServiceUrls:CategoriaService"]}/api/categorias/{categoriaId}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categoria = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                    return categoria?.GetValueOrDefault("nombre")?.ToString();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error al obtener nombre de categoría {categoriaId}");
                return null;
            }
        }
    }
}