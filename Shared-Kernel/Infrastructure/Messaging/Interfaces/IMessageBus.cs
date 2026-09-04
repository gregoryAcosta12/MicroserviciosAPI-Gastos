using System;
using Shared.Kernel.Infrastructure.Events;

namespace Shared.Kernel.Infrastructure.Messaging.Interfaces
{
    public interface IMessageBus
    {
        /// <summary>
        /// Publica un evento en el bus de mensajes
        /// </summary>
        Task Publish<T>(T @event) where T : IntegrationEvent;

        /// <summary>
        /// Suscribe un manejador a un evento
        /// </summary>
        Task Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IEventHandler<T>;

        /// <summary>
        /// Desuscribe un manejador de un evento
        /// </summary>
        Task Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IEventHandler<T>;

        /// <summary>
        /// Verifica si el bus está conectado
        /// </summary>
        bool IsConnected { get; }
    }
}