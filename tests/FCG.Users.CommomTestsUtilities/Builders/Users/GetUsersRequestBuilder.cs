using Bogus;
using FCG.Users.Application.UseCases.Admin.GetUsers;

namespace FCG.Users.CommomTestsUtilities.Builders.Users
{
    public class GetUsersRequestBuilder
    {
        public GetUsersRequest Build()
        {
            var faker = new Faker();
            return new GetUsersRequest(
                PageNumber: 1,
                PageSize: 10,
                Name: null,
                Email: null);
        }

        public GetUsersRequest BuildWithCustomValues(int pageNumber, int pageSize, string? name = null, string? email = null)
        {
            return new GetUsersRequest(pageNumber, pageSize, name, email);
        }

        public GetUsersRequest BuildWithFilters(string? name = null, string? email = null)
        {
            return new GetUsersRequest(1, 10, name, email);
        }
    }
}
