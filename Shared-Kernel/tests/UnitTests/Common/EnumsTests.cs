using FluentAssertions;
using Shared.Kernel.Common.Enums;
using Xunit;

namespace Shared.Kernel.Tests.UnitTests.Common
{
    public class EnumsTests
    {
        [Fact]
        public void EstadoGasto_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert
            ((int)EstadoGasto.Pendiente).Should().Be(1);
            ((int)EstadoGasto.Pagado).Should().Be(2);
            ((int)EstadoGasto.Cancelado).Should().Be(3);
        }

        [Fact]
        public void EstadoGasto_ToDisplayString_ShouldReturnCorrectString()
        {
            // Arrange & Act & Assert
            EstadoGasto.Pendiente.ToDisplayString().Should().Be("Pendiente");
            EstadoGasto.Pagado.ToDisplayString().Should().Be("Pagado");
            EstadoGasto.Cancelado.ToDisplayString().Should().Be("Cancelado");
        }

        [Fact]
        public void EstadoGasto_FromString_ShouldReturnCorrectEnum()
        {
            // Arrange & Act
            var pendiente = EstadoGastoExtensions.FromString("pendiente");
            var pagado = EstadoGastoExtensions.FromString("pagado");
            var cancelado = EstadoGastoExtensions.FromString("cancelado");

            // Assert
            pendiente.Should().Be(EstadoGasto.Pendiente);
            pagado.Should().Be(EstadoGasto.Pagado);
            cancelado.Should().Be(EstadoGasto.Cancelado);
        }

        [Fact]
        public void EstadoGasto_FromString_WithInvalidValue_ShouldReturnPendiente()
        {
            // Arrange & Act
            var result = EstadoGastoExtensions.FromString("invalido");

            // Assert
            result.Should().Be(EstadoGasto.Pendiente);
        }

        [Fact]
        public void MetodoPago_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert
            ((int)MetodoPago.Efectivo).Should().Be(1);
            ((int)MetodoPago.TarjetaCredito).Should().Be(2);
            ((int)MetodoPago.TarjetaDebito).Should().Be(3);
            ((int)MetodoPago.Transferencia).Should().Be(4);
            ((int)MetodoPago.PayPal).Should().Be(5);
            ((int)MetodoPago.Otro).Should().Be(6);
        }

        [Fact]
        public void MetodoPago_ToDisplayString_ShouldReturnCorrectString()
        {
            // Arrange & Act & Assert
            MetodoPago.Efectivo.ToDisplayString().Should().Be("Efectivo");
            MetodoPago.TarjetaCredito.ToDisplayString().Should().Be("Tarjeta de Crédito");
            MetodoPago.TarjetaDebito.ToDisplayString().Should().Be("Tarjeta de Débito");
            MetodoPago.Transferencia.ToDisplayString().Should().Be("Transferencia Bancaria");
            MetodoPago.PayPal.ToDisplayString().Should().Be("PayPal");
            MetodoPago.Otro.ToDisplayString().Should().Be("Otro");
        }

        [Theory]
        [InlineData("efectivo", MetodoPago.Efectivo)]
        [InlineData("tarjetacredito", MetodoPago.TarjetaCredito)]
        [InlineData("tarjetadebito", MetodoPago.TarjetaDebito)]
        [InlineData("transferencia", MetodoPago.Transferencia)]
        [InlineData("paypal", MetodoPago.PayPal)]
        public void MetodoPago_FromString_ShouldReturnCorrectEnum(string input, MetodoPago expected)
        {
            // Arrange & Act
            var result = MetodoPagoExtensions.FromString(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void TipoGasto_ShouldHaveCorrectValues()
        {
            // Arrange & Act & Assert
            ((int)TipoGasto.Fijo).Should().Be(1);
            ((int)TipoGasto.Variable).Should().Be(2);
            ((int)TipoGasto.Extraordinario).Should().Be(3);
        }

        [Fact]
        public void TipoGasto_ToDisplayString_ShouldReturnCorrectString()
        {
            // Arrange & Act & Assert
            TipoGasto.Fijo.ToDisplayString().Should().Be("Fijo");
            TipoGasto.Variable.ToDisplayString().Should().Be("Variable");
            TipoGasto.Extraordinario.ToDisplayString().Should().Be("Extraordinario");
        }

        [Theory]
        [InlineData("fijo", TipoGasto.Fijo)]
        [InlineData("variable", TipoGasto.Variable)]
        [InlineData("extraordinario", TipoGasto.Extraordinario)]
        public void TipoGasto_FromString_ShouldReturnCorrectEnum(string input, TipoGasto expected)
        {
            // Arrange & Act
            var result = TipoGastoExtensions.FromString(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}