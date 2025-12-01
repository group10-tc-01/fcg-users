using FCG.Users.Application.UseCases.Users.UpdatePassword;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Users.UpdatePassword
{
    public class UpdatePasswordRequestValidatorTests
    {
        private readonly UpdatePasswordRequestValidator _validator;
        private readonly UpdatePasswordRequestBuilder _builder;

        public UpdatePasswordRequestValidatorTests()
        {
            _validator = new UpdatePasswordRequestValidator();
            _builder = new UpdatePasswordRequestBuilder();
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
        public void Given_InvalidNewPassword_When_Validate_Then_ShouldFailValidation(string? newPassword)
        {
            // Arrange
            var input = _builder.BuildWithCustomValues("CurrentPass@123", newPassword!);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdatePasswordRequest.NewPassword));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Given_InvalidCurrentPassword_When_Validate_Then_ShouldFailValidation(string? currentPassword)
        {
            // Arrange
            var input = _builder.BuildWithCustomValues(currentPassword!, "NewPass@123");

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdatePasswordRequest.CurrentPassword));
        }

        [Fact]
        public void Given_SameCurrentAndNewPassword_When_Validate_Then_ShouldFailValidation()
        {
            // Arrange
            var password = "SamePass@123";
            var input = _builder.BuildWithCustomValues(password, password);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
        }
    }
}
