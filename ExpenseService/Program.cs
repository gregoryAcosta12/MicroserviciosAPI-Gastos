using System.Text;
using Gateway.Interfaces;
using Gateway.Middleware;
using Gateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// 1. CONFIGURACIÓN DE LOGGING (Serilog)
// ========================================
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console()
          .WriteTo.File("logs/gateway-.txt", rollingInterval: RollingInterval.Day);
});

// ========================================
// 2. CONFIGURACIÓN DE OCELOT (Gateway)
// ========================================
builder.Configuration.AddJsonFile("Config/ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// ========================================
// 3. CONFIGURACIÓN DE AUTENTICACIÓN JWT
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
builder.Services.AddSwaggerGen();

// Servicios personalizados
builder.Services.AddScoped<IGatewayService, GatewayService>();

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
// 7. MIDDLEWARE PERSONALIZADO
// ========================================
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<AuthenticationMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// ========================================
// 8. PIPELINE DE LA APP
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

// ========================================
// 9. OCELOT GATEWAY
// ========================================
await app.UseOcelot();

// ========================================
// 10. HEALTH CHECKS
// ========================================
app.MapControllers();
app.MapHealthChecks("/health");

// ========================================
// 11. EJECUTAR APP
// ========================================
app.Run();