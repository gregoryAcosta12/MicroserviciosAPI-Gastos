using System.ComponentModel.DataAnnotations;

namespace API_Gasto_Service.DTOs.Requests
{
    public class GastoCreateDTO
    {
        [Required]
        [Range(0.01, 999999.99, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        public string? MetodoPago { get; set; }

        public string? Estado { get; set; } = "Pendiente";

        public bool EsRecurrente { get; set; } = false;

        public List<GastoDetalleDTO>? Detalles { get; set; }
    }

    public class GastoDetalleDTO
    {
        [Required]
        [MaxLength(100)]
        public string Campo { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Valor { get; set; } = string.Empty;
    }
}