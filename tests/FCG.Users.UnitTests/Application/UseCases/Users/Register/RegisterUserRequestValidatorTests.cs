using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Users.Register
{
    public class RegisterUserRequestValidatorTests
    {
        private readonly RegisterUserRequestValidator _validator;
        private readonly RegisterUserRequestBuilder _builder;

        public RegisterUserRequestValidatorTests()
        {
            _validator = new RegisterUserRequestValidator();
            _builder = new RegisterUserRequestBuilder();
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Given_InvalidName_When_Validate_Then_ShouldFailValidation(string? name)
        {
            // Arrange
            var input = _builder.BuildWithValues(name!, "test@example.com", "Password@123");

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserRequest.Name));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        public void Given_InvalidEmail_When_Validate_Then_ShouldFailValidation(string email)
        {
            // Arrange
            var input = _builder.BuildWithValues("Test User", email, "Password@123");

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserRequest.Email));
        }

        [Theory]
        [InlineData("short")]
        [InlineData("password")]
        [InlineData("PASSWORD123")]
        [InlineData("password123")]
        public void Given_InvalidPassword_When_Validate_Then_ShouldFailValidation(string password)
        {
            // Arrange
            var input = _builder.BuildWithValues("Test User", "test@example.com", password);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(RegisterUserRequest.Password));
        }
    }
}
