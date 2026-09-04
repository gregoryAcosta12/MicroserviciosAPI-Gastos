using System.Security.Claims;
using API_Gasto_Service.DTOs.Requests;
using API_Gasto_Service.DTOs.Responses;
using API_Gasto_Service.Services.Interfaces;
using Gasto_Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Gasto_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GastosController : ControllerBase
    {
        private readonly IGastoService _gastoService;
        private readonly IReporteService _reporteService;
        private readonly ILogger<GastosController> _logger;

        public GastosController(
            IGastoService gastoService,
            IReporteService reporteService,
            ILogger<GastosController> logger)
        {
            _gastoService = gastoService;
            _reporteService = reporteService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los gastos del usuario autenticado
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetGastos([FromQuery] GastoFilterDTO filter)
        {
            try
            {
                var usuarioId = GetUserId();
                filter.UsuarioId = usuarioId;

                var result = await _gastoService.GetFilteredAsync(filter);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener gastos");
                return StatusCode(500, new { Message = "Error al obtener gastos" });
            }
        }

        /// <summary>
        /// Obtiene un gasto por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGasto(int id)
        {
            try
            {
                var usuarioId = GetUserId();
                var gasto = await _gastoService.GetByIdAsync(id, usuarioId);

                if (gasto == null)
                    return NotFound(new { Message = "Gasto no encontrado" });

                return Ok(gasto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener gasto {id}");
                return StatusCode(500, new { Message = "Error al obtener gasto" });
            }
        }

        /// <summary>
        /// Crea un nuevo gasto
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateGasto([FromBody] GastoCreateDTO dto)
        {
            try
            {
                var usuarioId = GetUserId();
                var gasto = await _gastoService.CreateAsync(dto, usuarioId);

                return CreatedAtAction(nameof(GetGasto), new { id = gasto.Id }, gasto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear gasto");
                return StatusCode(500, new { Message = "Error al crear gasto" });
            }
        }

        /// <summary>
        /// Actualiza un gasto
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGasto(int id, [FromBody] GastoUpdateDTO dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { Message = "El ID no coincide" });

                var usuarioId = GetUserId();
                var gasto = await _gastoService.UpdateAsync(dto, usuarioId);

                if (gasto == null)
                    return NotFound(new { Message = "Gasto no encontrado" });

                return Ok(gasto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar gasto {id}");
                return StatusCode(500, new { Message = "Error al actualizar gasto" });
            }
        }

        /// <summary>
        /// Elimina un gasto
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGasto(int id)
        {
            try
            {
                var usuarioId = GetUserId();
                var result = await _gastoService.DeleteAsync(id, usuarioId);

                if (!result)
                    return NotFound(new { Message = "Gasto no encontrado" });

                return Ok(new { Message = "Gasto eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar gasto {id}");
                return StatusCode(500, new { Message = "Error al eliminar gasto" });
            }
        }

        /// <summary>
        /// Obtiene el resumen de gastos
        /// </summary>
        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen([FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            try
            {
                var usuarioId = GetUserId();
                var resumen = await _reporteService.GetResumenAsync(usuarioId, fechaInicio, fechaFin);
                return Ok(resumen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener resumen");
                return StatusCode(500, new { Message = "Error al obtener resumen" });
            }
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 1;
        }
    }
}