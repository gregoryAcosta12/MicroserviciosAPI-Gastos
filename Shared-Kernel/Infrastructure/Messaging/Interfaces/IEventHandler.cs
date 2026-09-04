using Shared.Kernel.Infrastructure.Events;

namespace Shared.Kernel.Infrastructure.Messaging.Interfaces
{
    public interface IEventHandler<T> where T : IntegrationEvent
    {
        /// <summary>
        /// Maneja un evento recibido
        /// </summary>
        Task Handle(T @event);
    }
}