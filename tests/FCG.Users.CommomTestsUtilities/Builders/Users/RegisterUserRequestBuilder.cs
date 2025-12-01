using Bogus;
using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.CommomTestsUtilities.Helpers;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public class RegisterUserRequestBuilder
    {
        public RegisterUserRequest Build()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(f.Name.FullName(), f.Internet.Email(), PasswordGenerator.GenerateValidPassword(f))).Generate();
        }

        public RegisterUserRequest BuildWithCustomValues(string name, string email, string password)
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(name, email, password)).Generate();
        }
    }
}
