namespace Shared.Kernel.Infrastructure.Events
{
    public class GastoCreadoEvent : IntegrationEvent
    {
        public int GastoId { get; set; }
        public decimal Monto { get; set; }
        public int CategoriaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set; }
        public string? Descripcion { get; set; }
        public string? Estado { get; set; }
    }
}