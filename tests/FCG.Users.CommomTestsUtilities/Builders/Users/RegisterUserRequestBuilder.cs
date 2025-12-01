using Bogus;
using FCG.Users.Application.UseCases.Users.Register;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public class RegisterUserRequestBuilder
    {
        public RegisterUserRequest Build()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(f.Name.FullName(), f.Internet.Email(), GenerateValidPassword(f))).Generate();
        }

        public RegisterUserRequest BuildWithEmptyName()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(string.Empty, f.Internet.Email(), GenerateValidPassword(f))).Generate();
        }

        public RegisterUserRequest BuildWithNullName()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(null!, f.Internet.Email(), GenerateValidPassword(f))).Generate();
        }

        public RegisterUserRequest BuildWithWhitespaceName()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest("   ", f.Internet.Email(), GenerateValidPassword(f))).Generate();
        }

        public RegisterUserRequest BuildWithInvalidEmail()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(f.Name.FullName(), f.Lorem.Word(), GenerateValidPassword(f))).Generate();
        }

        public RegisterUserRequest BuildWithShortPassword()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(f.Name.FullName(), f.Internet.Email(), "short")).Generate();
        }

        public RegisterUserRequest BuildWithPasswordWithoutUppercase()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(f.Name.FullName(), f.Internet.Email(), "password123")).Generate();
        }

        public RegisterUserRequest BuildWithPasswordWithoutLowercase()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(f.Name.FullName(), f.Internet.Email(), "PASSWORD123")).Generate();
        }

        public RegisterUserRequest BuildWithPasswordWithoutSpecialChar()
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(f.Name.FullName(), f.Internet.Email(), "password")).Generate();
        }

        public RegisterUserRequest BuildWithValues(string name, string email, string password)
        {
            return new Faker<RegisterUserRequest>()
                       .CustomInstantiator(f => new RegisterUserRequest(name, email, password)).Generate();
        }

        private static string GenerateValidPassword(Faker faker)
        {
            var letter = faker.Random.Char('a', 'z');
            var digit = faker.Random.Char('0', '9');
            var special = faker.PickRandom('!', '@', '#', '$', '%', '^', '&', '*');

            var additionalChars = faker.Random.String2(5, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*");

            var passwordChars = new[] { letter, digit, special }.Concat(additionalChars.ToCharArray()).ToArray();

            for (int i = passwordChars.Length - 1; i > 0; i--)
            {
                int j = faker.Random.Int(0, i);
                (passwordChars[i], passwordChars[j]) = (passwordChars[j], passwordChars[i]);
            }

            return new string(passwordChars);
        }
    }
}
