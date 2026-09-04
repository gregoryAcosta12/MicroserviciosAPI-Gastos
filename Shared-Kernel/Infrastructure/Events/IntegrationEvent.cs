namespace Shared.Kernel.Infrastructure.Events
{
    public abstract class IntegrationEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        public string EventType => GetType().Name;
    }
}