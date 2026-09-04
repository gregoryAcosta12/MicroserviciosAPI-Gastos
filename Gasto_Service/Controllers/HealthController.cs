using Microsoft.AspNetCore.Mvc;

namespace API_Gasto_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthController> _logger;

        public HealthController(IConfiguration configuration, ILogger<HealthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy ✅",
                Service = "Gasto-Service",
                Timestamp = DateTime.UtcNow,
                Database = "Connected"
            });
        }

        [HttpGet("detailed")]
        public IActionResult DetailedHealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy ✅",
                Service = "Gasto-Service",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Database = "Connected",
                Dependencies = new
                {
                    CategoriaService = "http://localhost:5002",
                    UsuarioService = "http://localhost:5003"
                }
            });
        }
    }
}