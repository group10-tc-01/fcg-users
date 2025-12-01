using Bogus;
using FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken;

namespace FCG.Users.CommomTestsUtilities.Builders.Authentication
{
    public class RefreshTokenRequestBuilder
    {
        private string _refreshToken = new Faker().Random.AlphaNumeric(30);

        public RefreshTokenRequest Build()
        {
            return new RefreshTokenRequest(_refreshToken);
        }
    }
}
