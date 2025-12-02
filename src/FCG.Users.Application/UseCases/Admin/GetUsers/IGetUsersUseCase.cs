using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Pagination;

namespace FCG.Users.Application.UseCases.Admin.GetUsers
{
    public interface IGetUsersUseCase : IQueryHandler<GetUsersRequest, PagedListResponse<GetUsersResponse>> { }
}
