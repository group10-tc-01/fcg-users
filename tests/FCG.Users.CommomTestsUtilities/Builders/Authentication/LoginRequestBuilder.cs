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

        public LoginRequest BuildWithEmptyEmail()
        {
            return new Faker<LoginRequest>()
                .CustomInstantiator(faker => new LoginRequest(
                    string.Empty,
                    faker.Internet.Password()))
                .Generate();
        }

        public LoginRequest BuildWithInvalidEmail()
        {
            return new Faker<LoginRequest>()
                .CustomInstantiator(faker => new LoginRequest(
                    faker.Lorem.Word(),
                    faker.Internet.Password()))
                .Generate();
        }

        public LoginRequest BuildWithEmptyPassword()
        {
            return new Faker<LoginRequest>()
                .CustomInstantiator(faker => new LoginRequest(
                    faker.Internet.Email(),
                    string.Empty))
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