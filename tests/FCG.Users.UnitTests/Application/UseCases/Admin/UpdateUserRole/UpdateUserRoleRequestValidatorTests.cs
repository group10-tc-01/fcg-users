using FCG.Users.Application.UseCases.Admin.UpdateUserRole;
using FCG.Users.CommomTestsUtilities.Builders.Admin;
using FCG.Users.Domain.Users;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Admin.UpdateUserRole
{
    public class UpdateUserRoleRequestValidatorTests
    {
        private readonly UpdateUserRoleRequestValidator _validator;
        private readonly UpdateUserRoleRequestBuilder _builder;

        public UpdateUserRoleRequestValidatorTests()
        {
            _validator = new UpdateUserRoleRequestValidator();
            _builder = new UpdateUserRoleRequestBuilder();
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
        public void Given_EmptyUserId_When_Validate_Then_ShouldFailValidation()
        {
            // Arrange
            var input = _builder.BuildWithCustomValues(Guid.Empty, Role.Admin);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateUserRoleRequest.Id));
        }

        [Fact]
        public void Given_InvalidRole_When_Validate_Then_ShouldFailValidation()
        {
            // Arrange
            var input = _builder.BuildWithCustomValues(Guid.NewGuid(), (Role)999);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateUserRoleRequest.NewRole));
        }

        [Theory]
        [InlineData(Role.Admin)]
        [InlineData(Role.User)]
        public void Given_ValidRole_When_Validate_Then_ShouldPassValidation(Role role)
        {
            // Arrange
            var input = _builder.BuildWithCustomValues(Guid.NewGuid(), role);

            // Act
            var result = _validator.Validate(input);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
