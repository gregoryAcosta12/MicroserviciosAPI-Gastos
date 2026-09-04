using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Gasto_Service.Models
{
    [Table("GastoDetalles")]
    public class GastoDetalle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int GastoId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Campo { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Valor { get; set; } = string.Empty;

        // Relación
        [ForeignKey("GastoId")]
        public virtual Gasto? Gasto { get; set; }
    }
}