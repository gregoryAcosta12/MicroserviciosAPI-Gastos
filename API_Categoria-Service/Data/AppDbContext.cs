using System.Collections.Generic;
using System.Reflection.Emit;
using API_Categoria_Service.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Categoria_Service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Índices únicos
            modelBuilder.Entity<Categoria>()
                .HasIndex(c => new { c.UsuarioId, c.Nombre })
                .IsUnique();

            // Índice para búsquedas rápidas
            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.UsuarioId);

            // Seed de datos
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria
                {
                    Id = 1,
                    Nombre = "Comida",
                    Descripcion = "Gastos de alimentación",
                    UsuarioId = 1,
                    Color = "#28a745",
                    Icono = "fa-utensils",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Categoria
                {
                    Id = 2,
                    Nombre = "Transporte",
                    Descripcion = "Gastos de transporte",
                    UsuarioId = 1,
                    Color = "#007bff",
                    Icono = "fa-car",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Categoria
                {
                    Id = 3,
                    Nombre = "Entretenimiento",
                    Descripcion = "Gastos de ocio y entretenimiento",
                    UsuarioId = 1,
                    Color = "#ffc107",
                    Icono = "fa-film",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                },
                new Categoria
                {
                    Id = 4,
                    Nombre = "Comida",
                    Descripcion = "Gastos de alimentación",
                    UsuarioId = 2,
                    Color = "#28a745",
                    Icono = "fa-utensils",
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                }
            );
        }
    }
}