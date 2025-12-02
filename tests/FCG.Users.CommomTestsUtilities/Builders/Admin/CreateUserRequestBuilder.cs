using Bogus;
using FCG.Users.Application.UseCases.Admin.CreateUser;
using FCG.Users.CommomTestsUtilities.Helpers;
using FCG.Users.Domain.Users;

namespace FCG.Users.CommomTestsUtilities.Builders.Admin
{
    public class CreateUserRequestBuilder
    {
        public CreateUserRequest Build()
        {
            return new Faker<CreateUserRequest>()
                       .CustomInstantiator(f => new CreateUserRequest(
                           f.Name.FullName(), 
                           f.Internet.Email(), 
                           PasswordGenerator.GenerateValidPassword(f),
                           f.PickRandom<Role>())).Generate();
        }

        public CreateUserRequest BuildWithCustomValues(string name, string email, string password, Role role)
        {
            return new Faker<CreateUserRequest>()
                       .CustomInstantiator(f => new CreateUserRequest(name, email, password, role)).Generate();
        }
    }
}
