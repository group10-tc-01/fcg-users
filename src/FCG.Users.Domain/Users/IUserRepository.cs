namespace FCG.Users.Domain.Users
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(IEnumerable<User> users, int totalCount)> GetPagedUsersAsync(
            int pageNumber,
            int pageSize,
            string? name,
            string? email,
            CancellationToken cancellationToken = default);
    }
}
