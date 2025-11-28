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
    }
}