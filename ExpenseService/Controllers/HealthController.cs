using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace API_Gateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthController> _logger;
        private readonly HttpClient _httpClient;

        public HealthController(IConfiguration configuration, ILogger<HealthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// Health check completo - verifica todos los servicios
        /// </summary>
        [HttpGet("full")]
        public async Task<IActionResult> FullHealthCheck()
        {
            var services = new Dictionary<string, string>();
            var allHealthy = true;

            // Verificar Gasto-Service
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5001/health");
                services["Gasto-Service"] = response.IsSuccessStatusCode ? "Healthy ✅" : "Unhealthy ❌";
                if (!response.IsSuccessStatusCode) allHealthy = false;
            }
            catch (Exception ex)
            {
                services["Gasto-Service"] = $"Down ❌: {ex.Message}";
                allHealthy = false;
            }

            // Verificar Categoria-Service
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5002/health");
                services["Categoria-Service"] = response.IsSuccessStatusCode ? "Healthy ✅" : "Unhealthy ❌";
                if (!response.IsSuccessStatusCode) allHealthy = false;
            }
            catch (Exception ex)
            {
                services["Categoria-Service"] = $"Down ❌: {ex.Message}";
                allHealthy = false;
            }

            // Verificar Usuario-Service
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:5003/health");
                services["Usuario-Service"] = response.IsSuccessStatusCode ? "Healthy ✅" : "Unhealthy ❌";
                if (!response.IsSuccessStatusCode) allHealthy = false;
            }
            catch (Exception ex)
            {
                services["Usuario-Service"] = $"Down ❌: {ex.Message}";
                allHealthy = false;
            }

            var result = new
            {
                Status = allHealthy ? "Healthy ✅" : "Unhealthy ❌",
                Timestamp = DateTime.UtcNow,
                Services = services
            };

            return allHealthy ? Ok(result) : StatusCode(503, result);
        }

        /// <summary>
        /// Health check simple - solo gateway
        /// </summary>
        [HttpGet]
        public IActionResult SimpleHealthCheck()
        {
            return Ok(new
            {
                Status = "Healthy ✅",
                Gateway = "Running",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}