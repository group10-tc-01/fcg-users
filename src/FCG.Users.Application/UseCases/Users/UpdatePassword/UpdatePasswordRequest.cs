using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Results;

namespace FCG.Users.Application.UseCases.Users.UpdatePassword
{
    public record UpdatePasswordRequest(string CurrentPassword, string NewPassword) : ICommand<Result<UpdatePasswordResponse>>;
}
