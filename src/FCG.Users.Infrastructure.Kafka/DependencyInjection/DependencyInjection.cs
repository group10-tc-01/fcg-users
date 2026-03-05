using Confluent.Kafka;
using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Infrastructure.Kafka.Producer;
using FCG.Users.Infrastructure.Kafka.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            var kafkaSettings = configuration.GetSection("KafkaSettings").Get<KafkaSettings>();

            ValidateKafkaSettings(kafkaSettings!);

            services.AddSingleton(kafkaSettings!);

            services.AddSingleton<IMessageProducer>(sp =>
            {
                var settings = sp.GetRequiredService<KafkaSettings>();
                var logger = sp.GetRequiredService<ILogger<KafkaProducer>>();
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = settings.BootstrapServers,
                    Acks = Acks.All,
                    EnableIdempotence = true,
                    MaxInFlight = 5,
                    MessageSendMaxRetries = 3
                };

                if (settings.UseSaslSsl)
                {
                    producerConfig.SecurityProtocol = SecurityProtocol.SaslSsl;
                    producerConfig.SaslMechanism = SaslMechanism.Plain;
                    producerConfig.SaslUsername = settings.SaslUsername;
                    producerConfig.SaslPassword = settings.SaslPassword;
                }

                return new KafkaProducer(producerConfig, logger);
            });

            return services;
        }

        private static void ValidateKafkaSettings(KafkaSettings settings)
        {
            if (settings.UseSaslSsl)
            {
                if (string.IsNullOrWhiteSpace(settings.SaslUsername))
                    throw new InvalidOperationException("KafkaSettings:SaslUsername must be configured when UseSaslSsl is true.");

                if (string.IsNullOrWhiteSpace(settings.SaslPassword))
                    throw new InvalidOperationException("KafkaSettings:SaslPassword must be configured when UseSaslSsl is true.");
            }
        }
    }
}
