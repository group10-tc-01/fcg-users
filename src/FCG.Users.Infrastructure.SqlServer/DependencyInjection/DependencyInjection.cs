using FCG.Users.Application.Abstractions.Audit;
using FCG.Users.Domain.Abstractions;
using FCG.Users.Domain.RefreshTokens;
using FCG.Users.Domain.Users;
using FCG.Users.Infrastructure.SqlServer.Persistance;
using FCG.Users.Infrastructure.SqlServer.Persistance.Interceptors;
using FCG.Users.Infrastructure.SqlServer.Persistance.Repositories;
using FCG.Users.Infrastructure.SqlServer.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.Infrastructure.SqlServer.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddSqlServerInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSqlServer(configuration);
            services.AddProviders();
            services.AddInterceptors();
            services.AddRepositories();

            return services;
        }

        private static void AddSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FcgUserDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }

        private static void AddProviders(this IServiceCollection services)
        {
            services.AddScoped<ICurrentSessionProvider, CurrentSessionProvider>();
        }

        private static void AddInterceptors(this IServiceCollection services)
        {
            services.AddScoped<AuditingInterceptor>();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<FcgUserDbContext>());
        }
    }
}
