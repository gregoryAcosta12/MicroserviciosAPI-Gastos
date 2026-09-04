using FluentAssertions;
using Shared.Kernel.Common.Constants;
using Shared_Kernel.Common.Constants;
using Xunit;

namespace Shared.Kernel.Tests.UnitTests.Common
{
    public class ConstantsTests
    {
        [Fact]
        public void ApplicationConstants_ShouldHaveValidServiceNames()
        {
            // Arrange & Act & Assert
            ApplicationConstants.GatewayService.Should().Be("API-Gateway");
            ApplicationConstants.GastoService.Should().Be("Gasto-Service");
            ApplicationConstants.CategoriaService.Should().Be("Categoria-Service");
            ApplicationConstants.UsuarioService.Should().Be("Usuario-Service");
        }

        [Fact]
        public void ApplicationConstants_ShouldHaveValidQueueNames()
        {
            // Arrange & Act & Assert
            ApplicationConstants.QueueGastoCreado.Should().Be("gasto-creado-queue");
            ApplicationConstants.QueueCategoriaActualizada.Should().Be("categoria-actualizada-queue");
            ApplicationConstants.QueueUsuarioCreado.Should().Be("usuario-creado-queue");
        }

        [Fact]
        public void ApplicationConstants_ShouldHaveValidExchangeNames()
        {
            // Arrange & Act & Assert
            ApplicationConstants.ExchangeGastos.Should().Be("gastos-exchange");
            ApplicationConstants.ExchangeCategorias.Should().Be("categorias-exchange");
            ApplicationConstants.ExchangeUsuarios.Should().Be("usuarios-exchange");
        }

        [Fact]
        public void ApplicationConstants_ShouldHaveValidRoles()
        {
            // Arrange & Act & Assert
            ApplicationConstants.RoleAdmin.Should().Be("Administrador");
            ApplicationConstants.RoleUser.Should().Be("Usuario");
        }

        [Fact]
        public void ApplicationConstants_ShouldHaveValidPaginationSettings()
        {
            // Arrange & Act & Assert
            ApplicationConstants.DefaultPageSize.Should().Be(10);
            ApplicationConstants.MaxPageSize.Should().Be(100);
        }

        [Theory]
        [InlineData("GatewayService", "API-Gateway")]
        [InlineData("GastoService", "Gasto-Service")]
        [InlineData("CategoriaService", "Categoria-Service")]
        [InlineData("UsuarioService", "Usuario-Service")]
        public void ApplicationConstants_ServiceNames_ShouldMatchExpected(string constantName, string expectedValue)
        {
            // Arrange
            var field = typeof(ApplicationConstants).GetField(constantName);

            // Act
            var actualValue = field?.GetValue(null)?.ToString();

            // Assert
            actualValue.Should().Be(expectedValue);
        }
    }

    public class ErrorMessagesTests
    {
        [Fact]
        public void ErrorMessages_ShouldHaveGeneralErrors()
        {
            // Arrange & Act & Assert
            ErrorMessages.InternalServerError.Should().Be("Ocurrió un error interno en el servidor");
            ErrorMessages.NotFound.Should().Be("Recurso no encontrado");
            ErrorMessages.Unauthorized.Should().Be("No autorizado");
            ErrorMessages.BadRequest.Should().Be("Solicitud inválida");
        }

        [Fact]
        public void ErrorMessages_ShouldHaveGastoErrors()
        {
            // Arrange & Act & Assert
            ErrorMessages.GastoNotFound.Should().Be("Gasto no encontrado");
            ErrorMessages.GastoCreateError.Should().Be("Error al crear el gasto");
            ErrorMessages.GastoInvalidMonto.Should().Be("El monto debe ser mayor a 0");
        }

        [Fact]
        public void ErrorMessages_ShouldHaveCategoriaErrors()
        {
            // Arrange & Act & Assert
            ErrorMessages.CategoriaNotFound.Should().Be("Categoría no encontrada");
            ErrorMessages.CategoriaExists.Should().Be("Ya existe una categoría con ese nombre");
            ErrorMessages.CategoriaHasGastos.Should().Be("No se puede eliminar la categoría porque tiene gastos asociados");
        }

        [Fact]
        public void ErrorMessages_ShouldHaveUserErrors()
        {
            // Arrange & Act & Assert
            ErrorMessages.UserNotFound.Should().Be("Usuario no encontrado");
            ErrorMessages.UserExists.Should().Be("El email ya está registrado");
            ErrorMessages.UserInvalidCredentials.Should().Be("Credenciales inválidas");
        }
    }
}