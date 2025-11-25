using MediatR;

namespace FCG.Users.Application.Abstractions.Messaging
{
    public interface IQuery<TResponse> : IRequest<TResponse> { }
}
