using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using API_Gasto_Service.Models;

namespace API_Gasto_Service.Data.Configurations
{
    public class GastoConfiguration : IEntityTypeConfiguration<Gasto>
    {
        public void Configure(EntityTypeBuilder<Gasto> builder)
        {
            // Índices
            builder.HasIndex(g => g.UsuarioId);
            builder.HasIndex(g => g.CategoriaId);
            builder.HasIndex(g => g.Fecha);
            builder.HasIndex(g => new { g.UsuarioId, g.Fecha });

            // Filtros
            builder.HasQueryFilter(g => g.Estado != "Cancelado");

            // Relaciones
            builder.HasMany(g => g.Detalles)
                   .WithOne(d => d.Gasto)
                   .HasForeignKey(d => d.GastoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}