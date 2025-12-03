using FCG.Users.Application.UseCases.Admin.DeactivateUser;

namespace FCG.Users.CommomTestsUtilities.Builders.Admin
{
    public class DeactivateUserRequestBuilder
    {
        public DeactivateUserRequest Build()
        {
            return new DeactivateUserRequest(Guid.NewGuid());
        }

        public DeactivateUserRequest BuildWithId(Guid id)
        {
            return new DeactivateUserRequest(id);
        }
    }
}
