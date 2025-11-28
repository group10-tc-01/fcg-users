using FCG.Users.Application.UseCases.Users.UpdatePassword;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public class UpdatePasswordRequestBuilder
    {
        public UpdatePasswordRequest Build(string currentPassword)
        {
            return new UpdatePasswordRequest(CurrentPassword: currentPassword, NewPassword: "UpdatedPass!2");
        }
    }
}
