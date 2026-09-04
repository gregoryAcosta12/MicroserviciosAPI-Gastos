using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Kernel.Infrastructure.Events;
using Shared.Kernel.Infrastructure.Messaging.Implementations;
using Shared.Kernel.Infrastructure.Messaging.Interfaces;
using Xunit;

namespace Shared.Kernel.Tests.IntegrationTests.Messaging
{
    public class RabbitMQIntegrationTests : IAsyncLifetime
    {
        private RabbitMQMessageBus? _messageBus;
        private readonly Mock<ILogger<RabbitMQMessageBus>> _loggerMock = new();

        public async Task InitializeAsync()
        {
            _messageBus = new RabbitMQMessageBus("localhost", "admin", "admin123", 5672, _loggerMock.Object);
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _messageBus?.Dispose();
            await Task.CompletedTask;
        }

        [Fact(Skip = "Requiere RabbitMQ corriendo")]
        public async Task PublishAndSubscribe_ShouldSendAndReceiveMessage()
        {
            // Arrange
            var eventReceived = new TaskCompletionSource<bool>();
            var testEvent = new TestIntegrationEvent
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow
            };

            // Act
            await _messageBus!.Subscribe<TestIntegrationEvent, TestEventHandler>();
            await _messageBus.Publish(testEvent);

            // Esperar un poco para que el mensaje sea procesado
            await Task.Delay(1000);

            // Assert
            // Verificar que el evento fue recibido
            var result = await Task.WhenAny(eventReceived.Task, Task.Delay(5000));
            result.Should().Be(eventReceived.Task);
        }

        [Fact(Skip = "Requiere RabbitMQ corriendo")]
        public async Task Publish_WithMultipleSubscribers_ShouldDeliverToAll()
        {
            // Arrange
            var eventReceived1 = new TaskCompletionSource<bool>();
            var eventReceived2 = new TaskCompletionSource<bool>();

            // Act
            await _messageBus!.Subscribe<TestIntegrationEvent, TestEventHandler>();
            await _messageBus.Subscribe<TestIntegrationEvent, TestEventHandler2>();

            await _messageBus.Publish(new TestIntegrationEvent());

            // Assert
            await Task.Delay(2000);
            // Ambos handlers deberían haber recibido el evento
        }

        [Fact(Skip = "Requiere RabbitMQ corriendo")]
        public async Task Publish_WithNoSubscribers_ShouldNotThrowException()
        {
            // Arrange & Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await _messageBus!.Publish(new TestIntegrationEvent());
            });

            // Assert
            exception.Should().BeNull();
        }
    }

    public class TestIntegrationEvent : IntegrationEvent
    {
    }

    public class TestEventHandler : IEventHandler<TestIntegrationEvent>
    {
        public Task Handle(TestIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }

    public class TestEventHandler2 : IEventHandler<TestIntegrationEvent>
    {
        public Task Handle(TestIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}