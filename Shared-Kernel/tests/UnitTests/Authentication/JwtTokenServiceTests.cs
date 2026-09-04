using FluentAssertions;
using Shared.Kernel.Authentication;
using Xunit;

namespace Shared.Kernel.Tests.UnitTests.Authentication
{
    public class JwtTokenServiceTests
    {
        private const string Secret = "MiClaveSecretaSuperSegura1234567890!@#$%";
        private const string Issuer = "TestIssuer";
        private const string Audience = "TestAudience";
        private const int ExpirationMinutes = 60;

        [Fact]
        public void JwtTokenService_GenerateToken_ShouldReturnValidToken()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, ExpirationMinutes);
            var userId = 1;
            var email = "test@email.com";
            var nombre = "Test User";
            var rol = "Usuario";

            // Act
            var token = service.GenerateToken(userId, email, nombre, rol);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Split('.').Length.Should().Be(3); // JWT tiene 3 partes
        }

        [Fact]
        public void JwtTokenService_ValidateToken_WithValidToken_ShouldReturnTrue()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, ExpirationMinutes);
            var token = service.GenerateToken(1, "test@email.com", "Test User");

            // Act
            var isValid = service.ValidateToken(token);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void JwtTokenService_ValidateToken_WithInvalidToken_ShouldReturnFalse()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, ExpirationMinutes);
            var invalidToken = "invalid.token.here";

            // Act
            var isValid = service.ValidateToken(invalidToken);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void JwtTokenService_GetUserIdFromToken_ShouldReturnCorrectUserId()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, ExpirationMinutes);
            var expectedUserId = 123;
            var token = service.GenerateToken(expectedUserId, "test@email.com", "Test User");

            // Act
            var userId = service.GetUserIdFromToken(token);

            // Assert
            userId.Should().Be(expectedUserId);
        }

        [Fact]
        public void JwtTokenService_GetUserIdFromToken_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, ExpirationMinutes);
            var invalidToken = "invalid.token.here";

            // Act
            var userId = service.GetUserIdFromToken(invalidToken);

            // Assert
            userId.Should().BeNull();
        }

        [Fact]
        public void JwtTokenService_GetEmailFromToken_ShouldReturnCorrectEmail()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, ExpirationMinutes);
            var expectedEmail = "test@email.com";
            var token = service.GenerateToken(1, expectedEmail, "Test User");

            // Act
            var email = service.GetEmailFromToken(token);

            // Assert
            email.Should().Be(expectedEmail);
        }

        [Fact]
        public void JwtTokenService_GetEmailFromToken_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, ExpirationMinutes);
            var invalidToken = "invalid.token.here";

            // Act
            var email = service.GetEmailFromToken(invalidToken);

            // Assert
            email.Should().BeNull();
        }

        [Fact]
        public void JwtTokenService_TokenExpiration_ShouldExpireAfterSetTime()
        {
            // Arrange
            var service = new JwtTokenService(Secret, Issuer, Audience, 1); // 1 minuto
            var token = service.GenerateToken(1, "test@email.com", "Test User");

            // Act
            Thread.Sleep(TimeSpan.FromSeconds(65)); // Esperar más de 1 minuto
            var isValid = service.ValidateToken(token);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}