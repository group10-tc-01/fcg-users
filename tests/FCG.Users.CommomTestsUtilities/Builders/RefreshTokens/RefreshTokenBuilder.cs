using Bogus;
using FCG.Users.Domain.RefreshTokens;

namespace FCG.Users.CommomTestsUtilities.Builders.RefreshTokens
{
    public class RefreshTokenBuilder
    {
        public RefreshToken Build()
        {
            return new Faker<RefreshToken>().CustomInstantiator(f => RefreshToken.Create(f.Random.AlphaNumeric(30), Guid.NewGuid(), TimeSpan.FromDays(7))).Generate();
        }

        public RefreshToken BuildExpired()
        {
            return new Faker<RefreshToken>().CustomInstantiator(f => RefreshToken.Create(f.Random.AlphaNumeric(30), Guid.NewGuid(), TimeSpan.FromSeconds(-1))).Generate();
        }

        public RefreshToken BuildWithUserId(Guid userId)
        {
            return new Faker<RefreshToken>().CustomInstantiator(f => RefreshToken.Create(f.Random.AlphaNumeric(30), userId, TimeSpan.FromDays(7))).Generate();
        }
    }
}
