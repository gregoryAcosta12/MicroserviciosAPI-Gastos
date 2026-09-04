using System.Collections.Generic;
using System.Reflection.Emit;
using API_Gasto_Service.Data.Configurations;
using API_Gasto_Service.Models;
using Gasto_Service.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace API_Gasto_Service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<GastoDetalle> GastoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones
            modelBuilder.ApplyConfiguration(new GastoConfiguration());

            // Seed de datos
            modelBuilder.Entity<Gasto>().HasData(
                new Gasto
                {
                    Id = 1,
                    Monto = 150.00m,
                    Descripcion = "Cena en restaurante",
                    Fecha = DateTime.UtcNow.AddDays(-5),
                    CategoriaId = 1,
                    UsuarioId = 1,
                    Estado = "Pagado",
                    FechaCreacion = DateTime.UtcNow.AddDays(-5)
                },
                new Gasto
                {
                    Id = 2,
                    Monto = 50.00m,
                    Descripcion = "Taxi al aeropuerto",
                    Fecha = DateTime.UtcNow.AddDays(-3),
                    CategoriaId = 2,
                    UsuarioId = 1,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.UtcNow.AddDays(-3)
                },
                new Gasto
                {
                    Id = 3,
                    Monto = 200.00m,
                    Descripcion = "Supermercado",
                    Fecha = DateTime.UtcNow.AddDays(-2),
                    CategoriaId = 1,
                    UsuarioId = 2,
                    Estado = "Pagado",
                    FechaCreacion = DateTime.UtcNow.AddDays(-2)
                }
            );
        }
    }
}