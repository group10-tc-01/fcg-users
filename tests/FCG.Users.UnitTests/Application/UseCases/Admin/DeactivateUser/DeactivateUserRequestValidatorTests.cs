using FCG.Users.Application.UseCases.Admin.DeactivateUser;
using FCG.Users.CommomTestsUtilities.Builders.Admin;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.DeactivateUser
{
    public class DeactivateUserRequestValidatorTests
    {
        private readonly DeactivateUserRequestValidator _validator;
        private readonly DeactivateUserRequestBuilder _builder;

        public DeactivateUserRequestValidatorTests()
        {
            _validator = new DeactivateUserRequestValidator();
            _builder = new DeactivateUserRequestBuilder();
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
        public void Given_EmptyId_When_Validate_Then_ShouldFailValidation()
        {
            // Arrange
            var input = _builder.BuildWithId(Guid.Empty);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(DeactivateUserRequest.Id));
        }

        [Fact]
        public void Given_ValidGuid_When_Validate_Then_ShouldPassValidation()
        {
            // Arrange
            var validId = Guid.NewGuid();
            var input = _builder.BuildWithId(validId);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}