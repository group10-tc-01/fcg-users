using Bogus;
using FCG.Users.Application.UseCases.Authentication.Login;

namespace FCG.Users.CommomTestsUtilities.Builders.Authentication
{
    public class LoginRequestBuilder
    {
        public LoginRequest BuildWithValues(string email, string password)
        {
            return new Faker<LoginRequest>()
                .CustomInstantiator(faker => new LoginRequest(email, password))
                .Generate();
        }
    }
}