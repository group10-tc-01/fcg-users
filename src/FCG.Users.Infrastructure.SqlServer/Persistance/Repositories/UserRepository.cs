using FCG.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.SqlServer.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        protected readonly FcgUserDbContext _fcgUserDbContext;

        public UserRepository(FcgUserDbContext dbContext)
        {
            _fcgUserDbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _fcgUserDbContext.User.AddAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _fcgUserDbContext.User.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _fcgUserDbContext.User.FirstOrDefaultAsync(u => u.IsActive && u.Id == id, cancellationToken);

            return user;
        }

        public async Task<(IEnumerable<User> users, int totalCount)> GetPagedUsersAsync(
            int pageNumber,
            int pageSize,
            string? name,
            string? email,
            CancellationToken cancellationToken = default)
        {
            var query = _fcgUserDbContext.User.AsNoTracking().Where(u => u.IsActive);

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(u => u.Name.Value.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(u => u.Email.Value.Contains(email));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }
    }
}