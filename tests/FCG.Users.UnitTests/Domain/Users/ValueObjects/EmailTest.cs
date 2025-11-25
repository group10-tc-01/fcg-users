using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FCG.Users.UnitTests.Domain.Users.ValueObjects
{
    public class EmailTest
    {
        [Fact]
        public void Given_ValidEmail_When_Create_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            string validEmail = "user@example.com";

            // Act
            var email = Email.Create(validEmail);

            // Assert
            email.Should().NotBeNull();
            email.Value.Should().Be(validEmail);
        }

        [Fact]
        public void Given_NullEmail_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string nullEmail = null!;
            var act = () => Email.Create(nullEmail!);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Email cannot be null or empty.");
        }

        [Fact]
        public void Given_EmptyEmail_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string emptyEmail = string.Empty;
            var act = () => Email.Create(emptyEmail);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Email cannot be null or empty.");
        }

        [Fact]
        public void Given_WhitespaceEmail_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string whitespaceEmail = " ";
            var act = () => Email.Create(whitespaceEmail);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Email cannot be null or empty.");
        }

        [Fact]
        public void Given_EmailLongerThan255Characters_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string longEmail = new string('a', 256) + "@example.com";
            var act = () => Email.Create(longEmail);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Email cannot exceed 255 characters.");
        }

        [Fact]
        public void Given_InvalidEmailFormat_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string invalidEmail = "invalid-email";
            var act = () => Email.Create(invalidEmail);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Invalid email format.");
        }

        [Fact]
        public void Given_EmailWithoutAtSymbol_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string emailWithoutAt = "userexample.com";
            var act = () => Email.Create(emailWithoutAt);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Invalid email format.");
        }

        [Fact]
        public void Given_EmailObject_When_ImplicitConvertToString_Then_ShouldReturnValue()
        {
            // Arrange
            var email = Email.Create("test@example.com");

            // Act
            string value = email;

            // Assert
            value.Should().Be("test@example.com");
        }

        [Fact]
        public void Given_StringValue_When_ImplicitConvertToEmail_Then_ShouldCreateEmail()
        {
            // Arrange
            string value = "convert@example.com";

            // Act
            Email email = value;

            // Assert
            email.Value.Should().Be("convert@example.com");
        }

        [Fact]
        public void Given_EmailObject_When_ToStringCalled_Then_ShouldReturnValue()
        {
            // Arrange
            var email = Email.Create("tostring@example.com");

            // Act
            string result = email.ToString();

            // Assert
            result.Should().Be("tostring@example.com");
        }
    }
}
