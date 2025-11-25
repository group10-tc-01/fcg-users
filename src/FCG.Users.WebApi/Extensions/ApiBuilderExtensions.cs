using FCG.Users.Infrastructure.SqlServer.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.WebApi.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ApiBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            using var dbContext = scope.ServiceProvider.GetRequiredService<FcgUserDbContext>();

            dbContext.Database.Migrate();
        }
    }
}
