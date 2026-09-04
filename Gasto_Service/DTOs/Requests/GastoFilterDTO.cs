namespace API_Gasto_Service.DTOs.Requests
{
    public class GastoFilterDTO
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? CategoriaId { get; set; }
        public decimal? MontoMinimo { get; set; }
        public decimal? MontoMaximo { get; set; }
        public string? Estado { get; set; }
        public int? UsuarioId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "Fecha";
        public bool SortDescending { get; set; } = true;
    }
}s