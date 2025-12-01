using Bogus;
using FCG.Users.Application.UseCases.Authentication.Login;
using FCG.Users.CommomTestsUtilities.Helpers;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public class LoginRequestValidatorBuilder
    {
        public LoginRequest Build()
        {
            return new Faker<LoginRequest>().CustomInstantiator(f => new LoginRequest(f.Internet.Email(), PasswordGenerator.GenerateValidPassword(f))).Generate();
        }
    }
}
