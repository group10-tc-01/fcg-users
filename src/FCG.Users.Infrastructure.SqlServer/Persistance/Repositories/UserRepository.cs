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
            await _fcgUserDbContext.Users.AddAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _fcgUserDbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Value == email, cancellationToken);
        }
    }
}