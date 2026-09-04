namespace API_Categoria_Service.DTOs
{
    public class CategoriaResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Color { get; set; }
        public string? Icono { get; set; }
        public bool Activo { get; set; }
        public int TotalGastos { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }

    public class CategoriaValidationDTO
    {
        public bool Existe { get; set; }
        public string? Nombre { get; set; }
    }
}