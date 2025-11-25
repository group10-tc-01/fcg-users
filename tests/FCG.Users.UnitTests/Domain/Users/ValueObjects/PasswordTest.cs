using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users.ValueObjects;
using FCG.Users.Messages;
using FluentAssertions;

namespace FCG.Users.UnitTests.Domain.Users.ValueObjects
{
    public class PasswordTest
    {
        [Fact]
        public void Given_ValidPassword_When_CreatePassword_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            string validPassword = "MySecure123!";

            // Act
            var password = Password.Create(validPassword);

            // Assert
            password.Should().NotBeNull();
            password.Value.Should().Be(validPassword);
        }

        [Fact]
        public void Given_NullPassword_When_CreatePassword_Then_ShouldThrowDomainException()
        {

            string nullPassword = null!;

            Action act = () => Password.Create(nullPassword);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PasswordCannotBeNullOrEmpty);
        }

        [Fact]
        public void Given_EmptyPassword_When_CreatePassword_Then_ShouldThrowDomainException()
        {
            string emptyPassword = string.Empty;

            Action act = () => Password.Create(emptyPassword);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PasswordCannotBeNullOrEmpty);
        }

        [Fact]
        public void Given_WhitespacePassword_When_CreatePassword_Then_ShouldThrowDomainException()
        {
            string whitespacePassword = "        ";

            Action act = () => Password.Create(whitespacePassword);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PasswordCannotBeNullOrEmpty);
        }

        [Fact]
        public void Given_ShortPassword_When_CreatePassword_Then_ShouldThrowDomainException()
        {
            string shortPassword = "Abc1!";

            Action act = () => Password.Create(shortPassword);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PasswordMinimumLength);
        }

        [Fact]
        public void Given_PasswordWithoutLetter_When_CreatePassword_Then_ShouldThrowDomainException()
        {
            string passwordWithoutLetter = "12345678!";

            Action act = () => Password.Create(passwordWithoutLetter);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PasswordMustContainLetter);
        }

        [Fact]
        public void Given_PasswordWithoutDigit_When_CreatePassword_Then_ShouldThrowDomainException()
        {
            string passwordWithoutDigit = "Password!";

            Action act = () => Password.Create(passwordWithoutDigit);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PasswordMustContainNumber);
        }

        [Fact]
        public void Given_PasswordWithoutSpecialCharacter_When_CreatePassword_Then_ShouldThrowDomainException()
        {
            string passwordWithoutSpecial = "Password123";

            Action act = () => Password.Create(passwordWithoutSpecial);

            act.Should().Throw<DomainException>().WithMessage(ResourceMessages.PasswordMustContainSpecialCharacter);
        }

        [Fact]
        public void Given_PasswordObject_When_CallToString_Then_ShouldReturnValue()
        {
            // Arrange
            var password = Password.Create("ToString123!");

            // Act
            string result = password.ToString();

            // Assert
            result.Should().Be("ToString123!");
        }
    }
}
