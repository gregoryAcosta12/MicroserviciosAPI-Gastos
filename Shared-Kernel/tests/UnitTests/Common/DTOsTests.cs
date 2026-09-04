using FluentAssertions;
using Shared.Kernel.Common.DTOs;
using Xunit;

namespace Shared.Kernel.Tests.UnitTests.Common
{
    public class DTOsTests
    {
        [Fact]
        public void BaseResponseDTO_Ok_ShouldReturnSuccessResponse()
        {
            // Arrange & Act
            var response = BaseResponseDTO.Ok("Test message");

            // Assert
            response.Success.Should().BeTrue();
            response.Message.Should().Be("Test message");
            response.Errors.Should().BeNull();
        }

        [Fact]
        public void BaseResponseDTO_Error_ShouldReturnErrorResponse()
        {
            // Arrange & Act
            var errors = new List<string> { "Error 1", "Error 2" };
            var response = BaseResponseDTO.Error("Test error", errors);

            // Assert
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Test error");
            response.Errors.Should().BeEquivalentTo(errors);
        }

        [Fact]
        public void BaseResponseDTO_Ok_WithData_ShouldReturnSuccessWithData()
        {
            // Arrange
            var data = new { Id = 1, Name = "Test" };

            // Act
            var response = BaseResponseDTO<object>.Ok(data, "Success");

            // Assert
            response.Success.Should().BeTrue();
            response.Message.Should().Be("Success");
            response.Data.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void BaseResponseDTO_Error_WithData_ShouldReturnErrorWithData()
        {
            // Arrange
            var errors = new List<string> { "Error" };

            // Act
            var response = BaseResponseDTO<object>.Error("Error message", errors);

            // Assert
            response.Success.Should().BeFalse();
            response.Message.Should().Be("Error message");
            response.Errors.Should().BeEquivalentTo(errors);
            response.Data.Should().BeNull();
        }

        [Fact]
        public void PaginatedResponseDTO_ShouldCalculateTotalPagesCorrectly()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 4, 5 };
            var totalCount = 25;
            var pageNumber = 2;
            var pageSize = 10;

            // Act
            var response = new PaginatedResponseDTO<int>(items, totalCount, pageNumber, pageSize);

            // Assert
            response.Items.Should().BeEquivalentTo(items);
            response.TotalCount.Should().Be(25);
            response.PageNumber.Should().Be(2);
            response.PageSize.Should().Be(10);
            response.TotalPages.Should().Be(3);
            response.HasPreviousPage.Should().BeTrue();
            response.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void PaginatedResponseDTO_FirstPage_ShouldNotHavePreviousPage()
        {
            // Arrange
            var items = new List<string>();
            var totalCount = 5;
            var pageNumber = 1;
            var pageSize = 10;

            // Act
            var response = new PaginatedResponseDTO<string>(items, totalCount, pageNumber, pageSize);

            // Assert
            response.HasPreviousPage.Should().BeFalse();
            response.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void PaginatedResponseDTO_LastPage_ShouldNotHaveNextPage()
        {
            // Arrange
            var items = new List<string> { "a", "b" };
            var totalCount = 12;
            var pageNumber = 2;
            var pageSize = 10;

            // Act
            var response = new PaginatedResponseDTO<string>(items, totalCount, pageNumber, pageSize);

            // Assert
            response.HasPreviousPage.Should().BeTrue();
            response.HasNextPage.Should().BeFalse();
        }
    }
}