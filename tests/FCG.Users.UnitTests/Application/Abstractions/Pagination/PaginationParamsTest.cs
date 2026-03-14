using FCG.Users.Application.Abstractions.Pagination;
using FluentAssertions;
using Xunit;

namespace FCG.Users.UnitTests.Application.Abstractions.Pagination
{
    public class PaginationParamsTest
    {
        [Fact]
        public void Constructor_WithValidPageNumberAndPageSize_ShouldCreateInstance()
        {
            // Arrange & Act
            var paginationParams = new PaginationParams(pageNumber: 1, pageSize: 10);

            // Assert
            paginationParams.PageNumber.Should().Be(1);
            paginationParams.PageSize.Should().Be(10);
        }

        [Fact]
        public void Constructor_WithMaxPageSize_ShouldCreateInstance()
        {
            // Arrange & Act
            var paginationParams = new PaginationParams(pageNumber: 1, pageSize: 50);

            // Assert
            paginationParams.PageNumber.Should().Be(1);
            paginationParams.PageSize.Should().Be(50);
        }

        [Fact]
        public void Constructor_WithMinPageSize_ShouldCreateInstance()
        {
            // Arrange & Act
            var paginationParams = new PaginationParams(pageNumber: 1, pageSize: 1);

            // Assert
            paginationParams.PageNumber.Should().Be(1);
            paginationParams.PageSize.Should().Be(1);
        }

        [Fact]
        public void Constructor_WithLargePageNumber_ShouldCreateInstance()
        {
            // Arrange & Act
            var paginationParams = new PaginationParams(pageNumber: 1000, pageSize: 10);

            // Assert
            paginationParams.PageNumber.Should().Be(1000);
            paginationParams.PageSize.Should().Be(10);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_WithInvalidPageNumber_ShouldThrowArgumentException(int invalidPageNumber)
        {
            // Arrange & Act
            var act = () => new PaginationParams(pageNumber: invalidPageNumber, pageSize: 10);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithParameterName("pageNumber")
                .WithMessage("PageNumber must be greater than or equal to 1.*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(51)]
        [InlineData(100)]
        public void Constructor_WithInvalidPageSize_ShouldThrowArgumentException(int invalidPageSize)
        {
            // Arrange & Act
            var act = () => new PaginationParams(pageNumber: 1, pageSize: invalidPageSize);

            // Assert
            act.Should()
                .Throw<ArgumentException>()
                .WithParameterName("pageSize")
                .WithMessage("PageSize must be between 1 and 50.*");
        }

        [Fact]
        public void PaginationParams_IsRecord_ShouldSupportEquality()
        {
            // Arrange
            var params1 = new PaginationParams(pageNumber: 1, pageSize: 10);
            var params2 = new PaginationParams(pageNumber: 1, pageSize: 10);
            var params3 = new PaginationParams(pageNumber: 2, pageSize: 10);

            // Assert
            params1.Should().Be(params2);
            params1.Should().NotBe(params3);
        }
    }
}