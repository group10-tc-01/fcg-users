using FCG.Users.Application.UseCases.Admin.CreateUser;
using FCG.Users.CommomTestsUtilities.Builders.Admin;
using FCG.Users.Domain.Users;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.CreateUser
{
    public class CreateUserRequestValidatorTests
    {
        private readonly CreateUserRequestValidator _validator;
        private readonly CreateUserRequestBuilder _builder;

        public CreateUserRequestValidatorTests()
        {
            _validator = new CreateUserRequestValidator();
            _builder = new CreateUserRequestBuilder();
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
            var input = _builder.BuildWithCustomValues(name!, "test@example.com", "Password@123", Role.User);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserRequest.Name));
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        public void Given_InvalidEmail_When_Validate_Then_ShouldFailValidation(string email)
        {
            // Arrange
            var input = _builder.BuildWithCustomValues("Test User", email, "Password@123", Role.User);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserRequest.Email));
        }

        [Theory]
        [InlineData("short")]
        [InlineData("password")]
        [InlineData("PASSWORD123")]
        [InlineData("password123")]
        public void Given_InvalidPassword_When_Validate_Then_ShouldFailValidation(string password)
        {
            // Arrange
            var input = _builder.BuildWithCustomValues("Test User", "test@example.com", password, Role.User);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserRequest.Password));
        }

        [Fact]
        public void Given_InvalidRole_When_Validate_Then_ShouldFailValidation()
        {
            // Arrange
            var input = _builder.BuildWithCustomValues("Test User", "test@example.com", "Password@123", (Role)999);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateUserRequest.Role));
        }

        [Theory]
        [InlineData(Role.Admin)]
        [InlineData(Role.User)]
        public void Given_ValidRole_When_Validate_Then_ShouldPassValidation(Role role)
        {
            // Arrange
            var input = _builder.BuildWithCustomValues("Test User", "test@example.com", "Password@123", role);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
