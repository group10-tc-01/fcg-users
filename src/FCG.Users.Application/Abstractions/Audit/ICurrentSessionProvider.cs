namespace FCG.Users.Application.Abstractions.Audit
{
    public interface ICurrentSessionProvider
    {
        Guid? GetUserId();
    }
}