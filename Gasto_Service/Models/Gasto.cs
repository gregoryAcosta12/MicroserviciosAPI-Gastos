using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gasto_Service.Models;

namespace API_Gasto_Service.Models
{
    [Table("Gastos")]
    public class Gasto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [MaxLength(50)]
        public string? MetodoPago { get; set; }

        [MaxLength(50)]
        public string? Estado { get; set; } = "Pendiente";

        public bool EsRecurrente { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        public virtual ICollection<GastoDetalle>? Detalles { get; set; }
    }
}