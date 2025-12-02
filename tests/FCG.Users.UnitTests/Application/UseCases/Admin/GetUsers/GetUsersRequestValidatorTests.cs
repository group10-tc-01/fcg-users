using FCG.Users.Application.UseCases.Admin.GetUsers;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.GetUsers
{
    public class GetUsersRequestValidatorTests
    {
        private readonly GetUsersRequestValidator _validator;
        private readonly GetUsersRequestBuilder _builder;

        public GetUsersRequestValidatorTests()
        {
            _validator = new GetUsersRequestValidator();
            _builder = new GetUsersRequestBuilder();
        }

        [Fact]
        public void Given_ValidInput_When_Validate_Then_ShouldPassValidation()
        {
            // Arrange
            var input = _builder.Build();

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Given_ValidInputWithFilters_When_Validate_Then_ShouldPassValidation()
        {
            // Arrange
            var input = _builder.BuildWithFilters("John Doe", "john@example.com");

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Given_NameExceeds255Characters_When_Validate_Then_ShouldFailValidation()
        {
            // Arrange
            var longName = new string('a', 256);
            var input = _builder.BuildWithFilters(name: longName);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetUsersRequest.Name));
        }

        [Fact]
        public void Given_EmailExceeds255Characters_When_Validate_Then_ShouldFailValidation()
        {
            // Arrange
            var longEmail = new string('a', 256);
            var input = _builder.BuildWithFilters(email: longEmail);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetUsersRequest.Email));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("user.example.com")]
        public void Given_InvalidEmailFormat_When_Validate_Then_ShouldFailValidation(string invalidEmail)
        {
            // Arrange
            var input = _builder.BuildWithFilters(email: invalidEmail);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(GetUsersRequest.Email));
        }

        [Fact]
        public void Given_EmptyNameFilter_When_Validate_Then_ShouldPassValidation()
        {
            // Arrange
            var input = _builder.BuildWithFilters(name: "");

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Given_EmptyEmailFilter_When_Validate_Then_ShouldPassValidation()
        {
            // Arrange
            var input = _builder.BuildWithFilters(email: "");

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
