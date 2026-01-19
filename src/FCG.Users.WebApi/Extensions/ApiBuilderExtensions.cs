using BCrypt.Net;
using FCG.Users.Domain.Users;
using FCG.Users.Infrastructure.SqlServer.Persistance;
using FCG.Users.WebApi.Middlewares;
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

            SeedAdminUser(dbContext);
        }

        private static void SeedAdminUser(FcgUserDbContext dbContext)
        {
            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "admin123";
            const string adminName = "Admin";

            var adminExists = dbContext.User.Any(u => u.Email.Value == adminEmail);

            if (!adminExists)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(adminPassword);
                var admin = User.CreateAdminUser(adminName, adminEmail, hashedPassword);
                admin.ClearDomainEvents();

                dbContext.User.Add(admin);
                dbContext.SaveChanges();
            }
        }

        public static void UseCustomerExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
        }

        public static void UseGlobalCorrelationId(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalCorrelationIdMiddleware>();
        }
    }
}
