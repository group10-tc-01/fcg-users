using FCG.Users.Application.Abstractions.Authentication;
using static BCrypt.Net.BCrypt;

namespace FCG.Users.Infrastructure.Auth.Authentication
{
    public class PasswordEncrypterService : IPasswordEncrypterService
    {
        public string Encrypt(string password)
        {
            return HashPassword(password);
        }

        public bool IsValid(string password, string hashedPassword)
        {
            return Verify(password, hashedPassword);
        }
    }
}
