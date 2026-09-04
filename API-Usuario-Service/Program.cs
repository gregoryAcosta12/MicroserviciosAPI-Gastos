using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

// ========================================
// 1. CONFIGURACIÓN DE LOGGING (Serilog)
// ========================================
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console()
          .WriteTo.File("logs/usuario-service-.txt", rollingInterval: RollingInterval.Day);
});

// ========================================
// 2. CONFIGURACIÓN DE BASE DE DATOS
// ========================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================
// 3. CONFIGURACIÓN DE JWT
// ========================================
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"] ?? "MiClaveSecretaSuperSegura1234567890!@#$%");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ExpenseService",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ExpenseServiceUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ========================================
// 4. CONFIGURACIÓN DE SERVICIOS
// ========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Usuario Service",
        Version = "v1",
        Description = "Servicio de autenticación y gestión de usuarios"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Registro de servicios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UsuarioEventPublisher>();

// ========================================
// 5. CONFIGURACIÓN DE CORS
// ========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// ========================================
// 6. CONSTRUIR APP
// ========================================
var app = builder.Build();

// ========================================
// 7. PIPELINE DE LA APP
// ========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ========================================
// 8. HEALTH CHECK
// ========================================
app.MapGet("/health", () => new
{
    Status = "Healthy ✅",
    Service = "Usuario-Service",
    Timestamp = DateTime.UtcNow
});

// ========================================
// 9. EJECUTAR APP
// ========================================
app.Run();