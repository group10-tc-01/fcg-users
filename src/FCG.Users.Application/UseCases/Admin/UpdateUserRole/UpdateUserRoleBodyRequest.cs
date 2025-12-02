using FCG.Users.Domain.Users;

namespace FCG.Users.Application.UseCases.Admin.UpdateUserRole
{
    public record UpdateUserRoleBodyRequest(Role NewRole);
}
