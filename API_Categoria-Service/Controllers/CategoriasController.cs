using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using API_Categoria_Service.DTOs;
using API_Categoria_Service.Services.Interfaces;

namespace API_Categoria_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(ICategoriaService categoriaService, ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las categorías del usuario autenticado
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            try
            {
                var usuarioId = GetUserId();
                var categorias = await _categoriaService.GetByUsuarioIdAsync(usuarioId);
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías");
                return StatusCode(500, new { Message = "Error al obtener categorías" });
            }
        }

        /// <summary>
        /// Obtiene una categoría por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            try
            {
                var usuarioId = GetUserId();
                var categoria = await _categoriaService.GetByIdAsync(id, usuarioId);

                if (categoria == null)
                    return NotFound(new { Message = "Categoría no encontrada" });

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener categoría {id}");
                return StatusCode(500, new { Message = "Error al obtener categoría" });
            }
        }

        /// <summary>
        /// Valida si existe una categoría (endpoint público para otros servicios)
        /// </summary>
        [HttpGet("{id}/validar")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidarCategoria(int id)
        {
            try
            {
                var existe = await _categoriaService.ExistsAsync(id);
                var nombre = existe ? await _categoriaService.GetNombreAsync(id) : null;

                return Ok(new CategoriaValidationDTO
                {
                    Existe = existe,
                    Nombre = nombre
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al validar categoría {id}");
                return Ok(new CategoriaValidationDTO { Existe = false });
            }
        }

        /// <summary>
        /// Crea una nueva categoría
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCategoria([FromBody] CategoriaCreateDTO dto)
        {
            try
            {
                var usuarioId = GetUserId();
                var categoria = await _categoriaService.CreateAsync(dto, usuarioId);

                return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, categoria);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear categoría");
                return StatusCode(500, new { Message = "Error al crear categoría" });
            }
        }

        /// <summary>
        /// Actualiza una categoría
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] CategoriaUpdateDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { Message = "El ID no coincide" });

                var usuarioId = GetUserId();
                var categoria = await _categoriaService.UpdateAsync(dto, usuarioId);

                if (categoria == null)
                    return NotFound(new { Message = "Categoría no encontrada" });

                return Ok(categoria);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar categoría {id}");
                return StatusCode(500, new { Message = "Error al actualizar categoría" });
            }
        }

        /// <summary>
        /// Elimina una categoría
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                var usuarioId = GetUserId();
                var result = await _categoriaService.DeleteAsync(id, usuarioId);

                if (!result)
                    return NotFound(new { Message = "Categoría no encontrada" });

                return Ok(new { Message = "Categoría eliminada exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar categoría {id}");
                return StatusCode(500, new { Message = "Error al eliminar categoría" });
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 1;
        }
    }
}