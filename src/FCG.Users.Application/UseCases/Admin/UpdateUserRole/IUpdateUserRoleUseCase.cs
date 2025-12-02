using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Admin.UpdateUserRole
{
    public interface IUpdateUserRoleUseCase : ICommandHandler<UpdateUserRoleRequest, UpdateUserRoleResponse> { }
}
