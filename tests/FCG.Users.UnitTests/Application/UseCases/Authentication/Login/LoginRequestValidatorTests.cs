using FCG.Users.Application.UseCases.Authentication.Login;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FluentAssertions;

namespace FCG.Users.UnitTests.Application.UseCases.Authentication.Login
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator;
        private readonly LoginRequestValidatorBuilder _builder;

        public LoginRequestValidatorTests()
        {
            _validator = new LoginRequestValidator();
            _builder = new LoginRequestValidatorBuilder();
        }

        [Fact]
        public void Given_ValidRequest_When_Validate_Then_ShouldPassValidation()
        {
            // Arrange
            var request = _builder.Build();

            // Act
            var result = _validator.Validate(request);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
