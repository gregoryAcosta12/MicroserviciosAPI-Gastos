using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace API_Gateway.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        // Rutas públicas que no requieren autenticación
        private readonly HashSet<string> _publicPaths = new()
        {
            "/api/auth/login",
            "/api/auth/register",
            "/api/gateway/ping",
            "/health",
            "/api/health",
            "/api/gateway/status"
        };

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Si es una ruta pública, continuar sin autenticación
            if (IsPublicPath(path))
            {
                _logger.LogInformation($"Ruta pública: {path}");
                await _next(context);
                return;
            }

            // Obtener token del header
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning($"Intento de acceso sin token: {path}");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    Status = 401,
                    Message = "No autorizado: Token no proporcionado",
                    Timestamp = DateTime.UtcNow
                });
                return;
            }

            // Validar token
            if (!await ValidateTokenAsync(token))
            {
                _logger.LogWarning($"Token inválido: {path}");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new
                {
                    Status = 401,
                    Message = "No autorizado: Token inválido",
                    Timestamp = DateTime.UtcNow
                });
                return;
            }

            _logger.LogInformation($"Autenticación exitosa: {path}");
            await _next(context);
        }

        private bool IsPublicPath(string path)
        {
            return _publicPaths.Any(p => path.StartsWith(p));
        }

        private async Task<bool> ValidateTokenAsync(string token)
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
    }
}