using FCG.Users.Application.DependencyInjection;
using FCG.Users.Infrastructure.Auth.DependencyInjection;
using FCG.Users.Infrastructure.Kafka.DependencyInjection;
using FCG.Users.Infrastructure.SqlServer.DependencyInjection;
using FCG.Users.WebApi.DependencyInjection;
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

            builder.Services.AddWebApi(builder.Configuration);

            builder.Services.AddApplication();

            builder.Services.AddKafkaInfrastructure(builder.Configuration);

            builder.Services.AddSqlServerInfrastructure(builder.Configuration);

            builder.Services.AddAuthInfrastruture(builder.Configuration);

            var app = builder.Build();

            app.UseWebApiPipeline();

            app.Run();
        }
    }
}
