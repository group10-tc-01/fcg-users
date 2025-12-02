using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Application.Abstractions.Pagination;

namespace FCG.Users.Application.UseCases.Admin.GetUsers
{
    public sealed record GetUsersRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? Name = null,
        string? Email = null
    ) : IQuery<PagedListResponse<GetUsersResponse>>
    {
        public PaginationParams GetPaginationParams() => new(PageNumber, PageSize);
    }
}
