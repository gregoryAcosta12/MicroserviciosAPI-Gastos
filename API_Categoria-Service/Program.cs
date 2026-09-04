using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using API_Categoria_Service.Data;
using API_Categoria_Service.Repositories.Interfaces;
using API_Categoria_Service.Repositories.Implementations;
using API_Categoria_Service.Services.Interfaces;
using API_Categoria_Service.Services.Implementations;
using API_Categoria_Service.Publishers;

// ========================================
// 1. CONFIGURACIÓN DE LOGGING (Serilog)
// ========================================
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console()
          .WriteTo.File("logs/categoria-service-.txt", rollingInterval: RollingInterval.Day);
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
        Title = "API Categoria Service",
        Version = "v1",
        Description = "Servicio de gestión de categorías"
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
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// Services
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// Publishers
builder.Services.AddScoped<CategoriaEventPublisher>();

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
    Service = "Categoria-Service",
    Timestamp = DateTime.UtcNow
});

// ========================================
// 9. EJECUTAR APP
// ========================================
app.Run();