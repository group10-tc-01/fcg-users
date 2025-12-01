using Bogus;
using FCG.Users.Application.UseCases.Authentication.Login;

namespace FCG.Users.CommomTestsUtilities.Builders.Authentication
{
    public class LoginRequestBuilder
    {
        public LoginRequest Build()
        {
            return new Faker<LoginRequest>()
                .CustomInstantiator(faker => new LoginRequest(
                    faker.Internet.Email(),
                    faker.Internet.Password()))
                .Generate();
        }

        public LoginRequest BuildWithValues(string email, string password)
        {
            return new Faker<LoginRequest>()
                .CustomInstantiator(faker => new LoginRequest(email, password))
                .Generate();
        }
    }
}