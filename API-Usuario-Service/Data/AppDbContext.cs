using System.Collections.Generic;
using System.Reflection.Emit;
using API_Usuario_Service.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Usuario_Service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Índices únicos
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed de datos iniciales
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Admin",
                    Email = "admin@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Rol = "Administrador",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Juan Pérez",
                    Email = "juan@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
                    Rol = "Usuario",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                }
            );
        }
    }
}