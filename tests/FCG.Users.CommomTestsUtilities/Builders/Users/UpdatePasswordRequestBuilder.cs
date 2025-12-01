using Bogus;
using FCG.Users.Application.UseCases.Users.UpdatePassword;
using FCG.Users.CommomTestsUtilities.Helpers;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public class UpdatePasswordRequestBuilder
    {
        public UpdatePasswordRequest Build()
        {
            var faker = new Faker();
            return new UpdatePasswordRequest(
                CurrentPassword: PasswordGenerator.GenerateValidPassword(faker),
                NewPassword: PasswordGenerator.GenerateValidPassword(faker));
        }

        public UpdatePasswordRequest BuildWithCustomValues(string currentPassword, string newPassword)
        {
            return new UpdatePasswordRequest(currentPassword, newPassword);
        }
    }
}
