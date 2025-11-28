using FCG.Users.Application.Abstractions.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Authentication;
using FCG.Users.CommomTestsUtilities.Builders.Users;
using FCG.Users.Domain.Users;
using FCG.Users.Infrastructure.SqlServer.Persistance;
using FCG.Users.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.IntegratedTests.Configurations
{
    [ExcludeFromCodeCoverage]
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private DbConnection? _connection;
        public List<User> CreatedUsers { get; private set; } = [];

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test").ConfigureServices(services =>
            {
                RemoveEntityFrameworkServices(services);
                RemoveKafkaServices(services);
                RemovePasswordEncrypterService(services);

                _connection?.Dispose();
                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();

                services.AddDbContext<FcgUserDbContext>(options =>
                {
                    options.UseSqlite(_connection)
                            .EnableSensitiveDataLogging()
                            .EnableDetailedErrors();
                });

                EnsureDatabaseSeeded(services);
            });
        }

        private static void RemoveEntityFrameworkServices(IServiceCollection services)
        {
            var descriptorsToRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<FcgUserDbContext>) ||
                d.ServiceType == typeof(FcgUserDbContext) ||
                d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true)
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

        }

        private static void RemovePasswordEncrypterService(IServiceCollection services)
        {
            var passwordEncrypterService = services.Where(service => service.ServiceType == typeof(IPasswordEncrypterService));

            if (passwordEncrypterService.Any())
            {
                services.Remove(passwordEncrypterService.First());
            }

            services.AddScoped<IPasswordEncrypterService>(_ => PasswordEncrypterServiceBuilder.Build());
        }

        private static void RemoveKafkaServices(IServiceCollection services)
        {
            var kafkaDescriptorsToRemove = services.Where(d =>
                d.ServiceType.FullName?.Contains("Kafka") == true ||
                d.ImplementationType?.FullName?.Contains("Kafka") == true)
                .ToList();

            foreach (var descriptor in kafkaDescriptorsToRemove)
            {
                services.Remove(descriptor);
            }
        }

        private void EnsureDatabaseSeeded(IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FcgUserDbContext>();

            Log.Information("Seeding database for integrated tests");

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            StartDatabase(dbContext);
        }

        private void StartDatabase(FcgUserDbContext context)
        {
            var itemsQuantity = 2;

            Log.Information($"Creating {itemsQuantity} items for integrated test");

            CreatedUsers = CreateUser(context, itemsQuantity);
        }

        private List<User> CreateUser(FcgUserDbContext context, int itemsQuantity)
        {
            var users = new List<User>();

            for (int i = 1; i <= itemsQuantity; i++)
            {
                var user = new UserBuilder().Build();
                users.Add(user);
            }

            context.User.AddRange(users);
            context.SaveChanges();
            Log.Information("Created {Count} regular users", users.Count);

            return users;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
