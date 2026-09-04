using System.ComponentModel.DataAnnotations;

namespace API_Gasto_Service.DTOs.Requests
{
    public class GastoUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Monto { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public string? MetodoPago { get; set; }

        public string? Estado { get; set; }

        public bool EsRecurrente { get; set; }
    }
}