using FCG.Users.Domain.RefreshTokens;
using Microsoft.EntityFrameworkCore;

namespace FCG.Users.Infrastructure.SqlServer.Persistance.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly FcgUserDbContext _fcgUserDbContext;

        public RefreshTokenRepository(FcgUserDbContext fcgDbContext) => _fcgUserDbContext = fcgDbContext;

        public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
        {
            await _fcgUserDbContext.RefreshToken.AddAsync(refreshToken);
            return refreshToken;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _fcgUserDbContext.RefreshToken.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return await _fcgUserDbContext.RefreshToken.Where(rt => rt.UserId == userId).ToListAsync();
        }

    }
}
