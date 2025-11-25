namespace FCG.Users.Domain.Users
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
