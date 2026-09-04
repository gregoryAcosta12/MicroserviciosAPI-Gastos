using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using API_Usuario_Service.DTOs;
using API_Usuario_Service.Models;
using API_Usuario_Service.Repositories;
using API_Usuario_Service.Services.Interfaces;
using API_Usuario_Service.Publishers;

namespace API_Usuario_Service.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly UsuarioEventPublisher _eventPublisher;

        public AuthService(
            IUsuarioRepository usuarioRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            UsuarioEventPublisher eventPublisher)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDTO)
        {
            try
            {
                // Buscar usuario por email
                var usuario = await _usuarioRepository.GetByEmailAsync(loginDTO.Email);
                if (usuario == null)
                {
                    _logger.LogWarning($"Usuario no encontrado: {loginDTO.Email}");
                    return null;
                }

                // Verificar contraseña
                if (!BCrypt.Verify(loginDTO.Password, usuario.PasswordHash))
                {
                    _logger.LogWarning($"Contraseña incorrecta: {loginDTO.Email}");
                    return null;
                }

                // Verificar si está activo
                if (!usuario.Activo)
                {
                    _logger.LogWarning($"Usuario inactivo: {loginDTO.Email}");
                    return null;
                }

                // Actualizar fecha de último login
                await _usuarioRepository.UpdateLastLoginAsync(usuario.Id);

                // Generar token JWT
                var token = GenerateJwtToken(usuario);

                return new LoginResponseDTO
                {
                    Token = token,
                    Usuario = new UsuarioResponseDTO
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Email = usuario.Email,
                        Rol = usuario.Rol,
                        Activo = usuario.Activo,
                        FechaCreacion = usuario.FechaCreacion,
                        FechaUltimoLogin = usuario.FechaUltimoLogin
                    },
                    ExpiraEn = DateTime.UtcNow.AddMinutes(
                        double.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60")
                    )
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en login: {loginDTO.Email}");
                throw;
            }
        }

        public async Task<UsuarioResponseDTO?> RegisterAsync(RegisterDTO registerDTO)
        {
            try
            {
                // Verificar si el email ya existe
                var exists = await _usuarioRepository.ExistsByEmailAsync(registerDTO.Email);
                if (exists)
                {
                    _logger.LogWarning($"Email ya registrado: {registerDTO.Email}");
                    return null;
                }

                // Crear nuevo usuario
                var usuario = new Usuario
                {
                    Nombre = registerDTO.Nombre,
                    Email = registerDTO.Email,
                    PasswordHash = BCrypt.HashPassword(registerDTO.Password),
                    Rol = "Usuario",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                };

                await _usuarioRepository.AddAsync(usuario);

                // Publicar evento: Usuario Creado
                await _eventPublisher.PublishUsuarioCreadoAsync(usuario);

                return new UsuarioResponseDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = usuario.Rol,
                    Activo = usuario.Activo,
                    FechaCreacion = usuario.FechaCreacion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en registro: {registerDTO.Email}");
                throw;
            }
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "MiClaveSecretaSuperSegura1234567890!@#$%");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol ?? "Usuario"),
                new Claim("sub", usuario.Id.ToString()),
                new Claim("email", usuario.Email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["Jwt:ExpirationInMinutes"] ?? "60")
                ),
                Issuer = _configuration["Jwt:Issuer"] ?? "ExpenseService",
                Audience = _configuration["Jwt:Audience"] ?? "ExpenseServiceUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? "MiClaveSecretaSuperSegura1234567890!@#$%");

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

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}