namespace FCG.Users.Application.Abstractions.Authentication
{
    public interface IPasswordEncrypter
    {
        string Encrypt(string password);
        bool IsValid(string password, string hashedPassword);
    }
}
