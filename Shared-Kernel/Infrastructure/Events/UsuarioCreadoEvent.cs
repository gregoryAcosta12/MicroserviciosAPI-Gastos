namespace Shared.Kernel.Infrastructure.Events
{
    public class CategoriaCreadaEvent : IntegrationEvent
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string? Descripcion { get; set; }
        public string? Color { get; set; }
        public string? Icono { get; set; }
    }
}