namespace FCG.Users.Application.UseCases.Admin.GetUsers
{
    public record GetUsersResponse(Guid Id, string Name, string Email, string Role, DateTime CreatedAt);
}
