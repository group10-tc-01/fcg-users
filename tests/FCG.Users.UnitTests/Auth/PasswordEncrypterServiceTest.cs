using FCG.Users.Infrastructure.Auth.Authentication;
using FluentAssertions;

namespace FCG.Users.UnitTests.Auth
{
    public class PasswordEncrypterServiceTest
    {
        private readonly PasswordEncrypterService _sut;

        public PasswordEncrypterServiceTest()
        {
            _sut = new PasswordEncrypterService();
        }

        [Fact]
        public void Given_ValidPassword_When_Encrypt_Then_ShouldReturnHashedPassword()
        {
            // Arrange
            var password = "Password@123";

            // Act
            var hashedPassword = _sut.Encrypt(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password);
            hashedPassword.Length.Should().BeGreaterThan(20);
        }

        [Fact]
        public void Given_SamePassword_When_EncryptTwice_Then_ShouldReturnDifferentHashes()
        {
            // Arrange
            var password = "Password@123";

            // Act
            var hash1 = _sut.Encrypt(password);
            var hash2 = _sut.Encrypt(password);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void Given_ValidPasswordAndHash_When_IsValid_Then_ShouldReturnTrue()
        {
            // Arrange
            var password = "Password@123";
            var hashedPassword = _sut.Encrypt(password);

            // Act
            var isValid = _sut.IsValid(password, hashedPassword);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Given_InvalidPassword_When_IsValid_Then_ShouldReturnFalse()
        {
            // Arrange
            var password = "Password@123";
            var wrongPassword = "WrongPassword@123";
            var hashedPassword = _sut.Encrypt(password);

            // Act
            var isValid = _sut.IsValid(wrongPassword, hashedPassword);

            // Assert
            isValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData("short")]
        [InlineData("VeryLongPasswordThatExceedsNormalLimits123456789!@#$%")]
        public void Given_DifferentPasswordLengths_When_Encrypt_Then_ShouldHashSuccessfully(string password)
        {
            // Act
            var hashedPassword = _sut.Encrypt(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            _sut.IsValid(password, hashedPassword).Should().BeTrue();
        }
    }
}
