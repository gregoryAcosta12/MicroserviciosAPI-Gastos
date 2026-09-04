using Gateway.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly IGatewayService _gatewayService;
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(IGatewayService gatewayService, ILogger<GatewayController> logger)
        {
            _gatewayService = gatewayService;
            _logger = logger;
        }

        /// <summary>
        /// Verifica el estado del gateway
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            _logger.LogInformation("Verificando estado del gateway");

            var status = new
            {
                Status = "Running",
                Timestamp = DateTime.UtcNow,
                Services = await _gatewayService.GetServicesStatusAsync()
            };

            return Ok(status);
        }

        /// <summary>
        /// Obtiene información de las rutas configuradas
        /// </summary>
        [Authorize]
        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            var routes = _gatewayService.GetConfiguredRoutes();
            return Ok(routes);
        }

        /// <summary>
        /// Endpoint de prueba sin autenticación
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                Message = "Gateway is alive! 🚀",
                Timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Endpoint de prueba con autenticación
        /// </summary>
        [Authorize]
        [HttpGet("secure-ping")]
        public IActionResult SecurePing()
        {
            var userId = User.FindFirst("sub")?.Value;
            return Ok(new
            {
                Message = "Secure endpoint accessed! 🔐",
                UserId = userId,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}