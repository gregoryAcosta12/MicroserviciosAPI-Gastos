namespace API_Usuariusing Microsoft.AspNetCore.Authorization;
using API_Usuario_Service.DTOs;
using API_Usuario_Service.Services.Interfaces;
using global::API_Usuario_Service.DTOs;
using global::API_Usuario_Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Usuario_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioService usuarioService, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var usuarios = await _usuarioService.GetAllAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { Message = "Error al obtener usuarios" });
            }
        }

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            try
            {
                var usuario = await _usuarioService.GetByIdAsync(id);
                if (usuario == null)
                {
                    return NotFound(new { Message = "Usuario no encontrado" });
                }
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener usuario {id}");
                return StatusCode(500, new { Message = "Error al obtener usuario" });
            }
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst("sub")?.Value ?? "0");
                var usuario = await _usuarioService.GetByIdAsync(userId);

                if (usuario == null)
                {
                    return NotFound(new { Message = "Usuario no encontrado" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener perfil");
                return StatusCode(500, new { Message = "Error al obtener perfil" });
            }
        }

        /// <summary>
        /// Actualiza un usuario
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioResponseDTO usuarioDTO)
        {
            try
            {
                if (id != usuarioDTO.Id)
                {
                    return BadRequest(new { Message = "El ID no coincide" });
                }

                var usuario = await _usuarioService.UpdateAsync(usuarioDTO);
                if (usuario == null)
                {
                    return NotFound(new { Message = "Usuario no encontrado" });
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar usuario {id}");
                return StatusCode(500, new { Message = "Error al actualizar usuario" });
            }
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var result = await _usuarioService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { Message = "Usuario no encontrado" });
                }

                return Ok(new { Message = "Usuario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar usuario {id}");
                return StatusCode(500, new { Message = "Error al eliminar usuario" });
            }
        }
    }
}