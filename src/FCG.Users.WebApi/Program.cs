using FCG.Users.Application.DependencyInjection;
using FCG.Users.Infrastructure.Auth.DependencyInjection;
using FCG.Users.Infrastructure.Kafka.DependencyInjection;
using FCG.Users.Infrastructure.SqlServer.DependencyInjection;
using FCG.Users.WebApi.DependencyInjection;
using FCG.Users.WebApi.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.WebApi
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        protected Program() { }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddWebApi(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddKafkaInfrastructure(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddAuthInfrastruture(builder.Configuration);

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Application started successfully");
            logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

            if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
            {
                app.ApplyMigrations();
                logger.LogInformation("Migrations applied");
            }

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseElmahIo();
            app.UseCustomerExceptionHandler();
            app.UseGlobalCorrelationId();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                }

            });

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
