using FluentAssertions;
using Shared.Kernel.Infrastructure.Events;
using Xunit;

namespace Shared.Kernel.Tests.UnitTests.Infrastructure.Events
{
    public class IntegrationEventTests
    {
        [Fact]
        public void IntegrationEvent_ShouldHaveIdAndCreationDate()
        {
            // Arrange & Act
            var @event = new TestIntegrationEvent();

            // Assert
            @event.Id.Should().NotBeEmpty();
            @event.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void IntegrationEvent_EventType_ShouldReturnClassName()
        {
            // Arrange & Act
            var @event = new TestIntegrationEvent();

            // Assert
            @event.EventType.Should().Be("TestIntegrationEvent");
        }

        [Fact]
        public void GastoCreadoEvent_ShouldHaveAllProperties()
        {
            // Arrange
            var @event = new GastoCreadoEvent
            {
                GastoId = 1,
                Monto = 150.50m,
                CategoriaId = 2,
                UsuarioId = 3,
                Fecha = DateTime.UtcNow,
                Descripcion = "Test gasto",
                Estado = "Pagado"
            };

            // Assert
            @event.GastoId.Should().Be(1);
            @event.Monto.Should().Be(150.50m);
            @event.CategoriaId.Should().Be(2);
            @event.UsuarioId.Should().Be(3);
            @event.Descripcion.Should().Be("Test gasto");
            @event.Estado.Should().Be("Pagado");
        }

        [Fact]
        public void CategoriaActualizadaEvent_ShouldHaveAllProperties()
        {
            // Arrange
            var @event = new CategoriaActualizadaEvent
            {
                CategoriaId = 1,
                Nombre = "Test Categoria",
                UsuarioId = 2,
                Descripcion = "Test description",
                Color = "#007bff",
                Icono = "fa-tag",
                Activo = true
            };

            // Assert
            @event.CategoriaId.Should().Be(1);
            @event.Nombre.Should().Be("Test Categoria");
            @event.UsuarioId.Should().Be(2);
            @event.Descripcion.Should().Be("Test description");
            @event.Color.Should().Be("#007bff");
            @event.Icono.Should().Be("fa-tag");
            @event.Activo.Should().BeTrue();
        }

        [Fact]
        public void UsuarioCreadoEvent_ShouldHaveAllProperties()
        {
            // Arrange
            var @event = new UsuarioCreadoEvent
            {
                UsuarioId = 1,
                Email = "test@email.com",
                Nombre = "Test User",
                Rol = "Usuario",
                FechaCreacion = DateTime.UtcNow
            };

            // Assert
            @event.UsuarioId.Should().Be(1);
            @event.Email.Should().Be("test@email.com");
            @event.Nombre.Should().Be("Test User");
            @event.Rol.Should().Be("Usuario");
        }
    }

    public class TestIntegrationEvent : IntegrationEvent
    {
    }
}