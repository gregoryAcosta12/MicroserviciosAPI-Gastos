using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Categoria_Service.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [MaxLength(50)]
        public string? Color { get; set; } = "#007bff";

        [MaxLength(50)]
        public string? Icono { get; set; } = "fa-tag";

        public bool Activo { get; set; } = true;

        public int TotalGastos { get; set; } = 0;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }
    }
}