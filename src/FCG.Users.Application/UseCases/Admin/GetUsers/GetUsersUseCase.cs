using FCG.Users.Application.Abstractions.Pagination;
using FCG.Users.Domain.Users;
using Microsoft.Extensions.Logging;

namespace FCG.Users.Application.UseCases.Admin.GetUsers
{
    public class GetUsersUseCase : IGetUsersUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUsersUseCase> _logger;

        public GetUsersUseCase(IUserRepository userRepository, ILogger<GetUsersUseCase> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<PagedListResponse<GetUsersResponse>> Handle(GetUsersRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetUsersUseCase started with PageNumber: {PageNumber}, PageSize: {PageSize}, Name: {Name}, Email: {Email}",
                request.PageNumber, request.PageSize, request.Name ?? "null", request.Email ?? "null");

            var (users, totalCount) = await _userRepository.GetPagedUsersAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Name,
                    request.Email,
                    cancellationToken);

            var usersResponse = users.Select(user => new GetUsersResponse(
                user.Id,
                user.Name.Value,
                user.Email.Value,
                user.Role.ToString(),
                user.CreatedAt));

            var pagedResponse = new PagedListResponse<GetUsersResponse>(
                usersResponse,
                totalCount,
                request.PageNumber,
                request.PageSize);

            _logger.LogInformation("GetUsersUseCase completed successfully. Returned {Count} users out of {TotalCount}",
                pagedResponse.Items.Count, totalCount);

            return pagedResponse;
        }
    }
}
