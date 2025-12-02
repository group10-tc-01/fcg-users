using MediatR;

namespace FCG.Users.Application.Abstractions.Messaging
{
    public interface ICommand : IRequest { }

    public interface ICommand<TResponse> : IRequest<TResponse> { }
}
