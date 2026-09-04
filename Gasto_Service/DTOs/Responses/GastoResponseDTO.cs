namespace API_Gasto_Service.DTOs.Responses
{
    public class GastoResponseDTO
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int CategoriaId { get; set; }
        public string? NombreCategoria { get; set; }
        public int UsuarioId { get; set; }
        public string? MetodoPago { get; set; }
        public string? Estado { get; set; }
        public bool EsRecurrente { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public int TotalDetalles { get; set; }
    }
}