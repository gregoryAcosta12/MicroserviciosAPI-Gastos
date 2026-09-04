using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API_Usuario_Service.DTOs;
using API_Usuario_Service.Services.Interfaces;

namespace API_Usuario_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Inicia sesión de usuario
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                _logger.LogInformation($"Intento de login: {loginDTO.Email}");

                var response = await _authService.LoginAsync(loginDTO);

                if (response == null)
                {
                    return Unauthorized(new
                    {
                        Status = 401,
                        Message = "Credenciales inválidas"
                    });
                }

                _logger.LogInformation($"Login exitoso: {loginDTO.Email}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en login: {loginDTO.Email}");
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "Error interno al procesar el login"
                });
            }
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                _logger.LogInformation($"Intento de registro: {registerDTO.Email}");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var usuario = await _authService.RegisterAsync(registerDTO);

                if (usuario == null)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "El email ya está registrado"
                    });
                }

                _logger.LogInformation($"Registro exitoso: {registerDTO.Email}");
                return Ok(new
                {
                    Status = 200,
                    Message = "Usuario registrado exitosamente",
                    Usuario = usuario
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en registro: {registerDTO.Email}");
                return StatusCode(500, new
                {
                    Status = 500,
                    Message = "Error interno al procesar el registro"
                });
            }
        }

        /// <summary>
        /// Verifica si un token es válido
        /// </summary>
        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            return Ok(new
            {
                Status = 200,
                Message = "Token válido",
                UserId = User.FindFirst("sub")?.Value,
                Email = User.FindFirst("email")?.Value
            });
        }
    }
}