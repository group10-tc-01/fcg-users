using Bogus;
using FCG.Users.Application.UseCases.Admin.UpdateUserRole;
using FCG.Users.Domain.Users;

namespace FCG.Users.CommomTestsUtilities.Builders.Admin
{
    public class UpdateUserRoleRequestBuilder
    {
        public UpdateUserRoleRequest Build()
        {
            return new Faker<UpdateUserRoleRequest>()
                       .CustomInstantiator(f => new UpdateUserRoleRequest(
                           Guid.NewGuid(),
                           f.PickRandom<Role>())).Generate();
        }

        public UpdateUserRoleRequest BuildWithCustomValues(Guid id, Role newRole)
        {
            return new Faker<UpdateUserRoleRequest>()
                       .CustomInstantiator(f => new UpdateUserRoleRequest(id, newRole)).Generate();
        }
    }
}
