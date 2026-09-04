namespace API_Gasto_Service.DTOs.Responses
{
    public class GastoDetailDTO : GastoResponseDTO
    {
        public List<GastoDetalleDTO> Detalles { get; set; } = new();
    }

    public class GastoDetalleDTO
    {
        public int Id { get; set; }
        public string Campo { get; set; } = string.Empty;
        public string Valor { get; set; } = string.Empty;
    }
}