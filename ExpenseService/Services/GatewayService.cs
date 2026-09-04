using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using API_Gateway.Interfaces;

namespace API_Gateway.Services
{
    public class GatewayService : IGatewayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GatewayService> _logger;
        private readonly HttpClient _httpClient;

        public GatewayService(IConfiguration configuration, ILogger<GatewayService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<Dictionary<string, string>> GetServicesStatusAsync()
        {
            var status = new Dictionary<string, string>
            {
                ["Gateway"] = "Running ✅"
            };

            var serviceUrls = new Dictionary<string, string>
            {
                ["Gasto-Service"] = _configuration["ServiceUrls:GastoService"] ?? "http://localhost:5001",
                ["Categoria-Service"] = _configuration["ServiceUrls:CategoriaService"] ?? "http://localhost:5002",
                ["Usuario-Service"] = _configuration["ServiceUrls:UsuarioService"] ?? "http://localhost:5003"
            };

            foreach (var service in serviceUrls)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{service.Value}/health");
                    status[service.Key] = response.IsSuccessStatusCode ? "Healthy ✅" : "Unhealthy ❌";
                }
                catch (Exception ex)
                {
                    status[service.Key] = $"Down ❌: {ex.Message}";
                }
            }

            return status;
        }

        public object GetConfiguredRoutes()
        {
            // Aquí podrías leer el archivo ocelot.json y devolver las rutas
            var routes = new[]
            {
                new { Path = "/api/gastos", Service = "Gasto-Service", Methods = "GET, POST, PUT, DELETE" },
                new { Path = "/api/reportes", Service = "Gasto-Service", Methods = "GET" },
                new { Path = "/api/categorias", Service = "Categoria-Service", Methods = "GET, POST, PUT, DELETE" },
                new { Path = "/api/auth", Service = "Usuario-Service", Methods = "POST, GET" },
                new { Path = "/api/usuarios", Service = "Usuario-Service", Methods = "GET, PUT, DELETE" }
            };

            return new
            {
                Routes = routes,
                TotalRoutes = routes.Length,
                Timestamp = DateTime.UtcNow
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "MiClaveSecretaSuperSegura1234567890!@#$%");

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"] ?? "ExpenseService",
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"] ?? "ExpenseServiceUsers",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "nameid");

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}