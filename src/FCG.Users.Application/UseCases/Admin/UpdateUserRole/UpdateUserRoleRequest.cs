using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;
using FCG.Users.Domain.Users;

namespace FCG.Users.Application.UseCases.Admin.UpdateUserRole
{
    public record UpdateUserRoleRequest(Guid Id, Role NewRole) : ICommand<Result<UpdateUserRoleResponse>>;
}
