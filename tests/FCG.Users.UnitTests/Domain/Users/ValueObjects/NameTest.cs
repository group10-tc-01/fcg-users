using FCG.Users.Domain.Exceptions;
using FCG.Users.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FCG.Users.UnitTests.Domain.Users.ValueObjects
{
    public class NameTest
    {
        [Fact]
        public void Given_ValidName_When_Create_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            string validName = "John Doe";

            // Act
            var name = Name.Create(validName);

            // Assert
            name.Should().NotBeNull();
            name.Value.Should().Be(validName);
        }

        [Fact]
        public void Given_ShortName_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string shortName = "A";
            var act = () => Name.Create(shortName);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Name must be at least 2 characters long.");
        }

        [Fact]
        public void Given_NullName_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string? nullName = null;
            var act = () => Name.Create(nullName!);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Name cannot be null or empty.");
        }

        [Fact]
        public void Given_EmptyName_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string emptyName = string.Empty;
            var act = () => Name.Create(emptyName);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Name cannot be null or empty.");
        }

        [Fact]
        public void Given_WhitespaceOnlyName_When_Create_Then_ShouldThrowDomainException()
        {
            // Arrange
            string whitespaceName = "   ";
            var act = () => Name.Create(whitespaceName);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Name cannot be null or empty.");
        }

        [Fact]
        public void Given_VeryLongName_When_Create_Then_ShouldCreateSuccessfully()
        {
            // Arrange
            string longName = new string('A', 256);

            // Act
            var act = () => Name.Create(longName!);

            // Act & Assert
            act.Should().Throw<DomainException>().WithMessage("Name cannot exceed 255 characters.");
        }

        [Fact]
        public void Given_NameObject_When_ToStringCalled_Then_ShouldReturnValue()
        {
            // Arrange
            var name = Name.Create("John Doe");

            // Act
            string result = name.ToString();

            // Assert
            result.Should().Be("John Doe");
        }
    }
}
