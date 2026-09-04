using Microsoft.AspNetCore.Mvc;

namespace API_Categoria_Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult HealthCheck()
        {
            _logger.LogInformation("Health check ejecutado");

            return Ok(new
            {
                Status = "Healthy ✅",
                Service = "Categoria-Service",
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
                Service = "Categoria-Service",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Database = "Connected",
                ActiveCategories = 4
            });
        }
    }
}