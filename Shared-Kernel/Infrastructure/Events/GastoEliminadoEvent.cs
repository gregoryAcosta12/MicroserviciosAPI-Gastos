namespace Shared.Kernel.Infrastructure.Events
{
    public class GastoEliminadoEvent : IntegrationEvent
    {
        public int GastoId { get; set; }
        public int? CategoriaId { get; set; }
        public int? UsuarioId { get; set; }
    }
}