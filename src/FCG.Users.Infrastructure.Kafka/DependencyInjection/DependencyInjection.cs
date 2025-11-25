using Confluent.Kafka;
using FCG.Users.Infrastructure.Kafka.Abstractions;
using FCG.Users.Infrastructure.Kafka.Configuration;
using FCG.Users.Infrastructure.Kafka.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace FCG.Users.Infrastructure.Kafka.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjection
    {
        public static IServiceCollection AddKafkaInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
            });

            var kafkaSettings = new KafkaSettings
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"] ?? string.Empty,
                UserCreatedTopic = configuration["Kafka:UserCreatedTopic"] ?? string.Empty
            };

            services.AddSingleton(kafkaSettings);

            services.AddSingleton<IKafkaProducer>(sp =>
            {
                var settings = sp.GetRequiredService<KafkaSettings>();
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = settings.BootstrapServers,
                    Acks = Acks.All,
                    EnableIdempotence = true,
                    MaxInFlight = 5,
                    MessageSendMaxRetries = 3
                };

                return new KafkaProducer(producerConfig);
            });

            return services;
        }
    }
}
