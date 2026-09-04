using System.Text;
using API_Gasto_Service.Consumers;
using API_Gasto_Service.Data;
using API_Gasto_Service.Publishers;
using API_Gasto_Service.Repositories.Implementations;
using API_Gasto_Service.Repositories.Interfaces;
using API_Gasto_Service.Services.Implementations;
using API_Gasto_Service.Services.Interfaces;
using FluentValidation.AspNetCore;
using Gasto_Service.Publishers;
using Gasto_Service.Repositories.Implementations;
using Gasto_Service.Repositories.Interfaces;
using Gasto_Service.Services.Implementations;
using Gasto_Service.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

// ========================================
// 1. CONFIGURACIÓN DE LOGGING (Serilog)
// ========================================
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console()
          .WriteTo.File("logs/gasto-service-.txt", rollingInterval: RollingInterval.Day);
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
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gasto Service",
        Version = "v1",
        Description = "Servicio de gestión de gastos y reportes"
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

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Repositories
builder.Services.AddScoped<IGastoRepository, GastoRepository>();

// Services
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

// Publishers
builder.Services.AddScoped<GastoEventPublisher>();

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
    Service = "Gasto-Service",
    Timestamp = DateTime.UtcNow
});

// ========================================
// 9. EJECUTAR APP
// ========================================
app.Run();