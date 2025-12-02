using Bogus;
using FCG.Users.CommomTestsUtilities.Helpers;
using FCG.Users.Domain.Users;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public class UserBuilder
    {
        public User Build()
        {
            return new Faker<User>().CustomInstantiator(f => User.CreateRegularUser(f.Name.FullName(), f.Internet.Email(), PasswordGenerator.GenerateValidPassword(f))).Generate();
        }
    }
}
