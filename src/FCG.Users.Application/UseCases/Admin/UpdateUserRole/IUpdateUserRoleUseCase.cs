using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Admin.UpdateUserRole
{
    public interface IUpdateUserRoleUseCase : ICommandHandler<UpdateUserRoleRequest, Result<UpdateUserRoleResponse>> { }
}
