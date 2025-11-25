using FCG.Users.Application.DependencyInjection;
using FCG.Users.Infrastructure.Kafka.DependencyInjection;
using FCG.Users.Infrastructure.SqlServer.DependencyInjection;
using FCG.Users.WebApi.DependencyInjection;
using FCG.Users.WebApi.Extensions;
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
            builder.Services.AddSwaggerGen();

            builder.Services.AddWebApi();
            builder.Services.AddApplication();
            builder.Services.AddKafkaInfrastructure(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.ApplyMigrations();
            }

            app.UseCustomerExceptionHandler();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}
