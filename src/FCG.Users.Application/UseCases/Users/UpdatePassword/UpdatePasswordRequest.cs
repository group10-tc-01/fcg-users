using FCG.Users.Application.Abstractions.Messaging;

namespace FCG.Users.Application.UseCases.Users.UpdatePassword
{
    public record UpdatePasswordRequest(string CurrentPassword, string NewPassword) : ICommand<UpdatePasswordResponse>;
}
