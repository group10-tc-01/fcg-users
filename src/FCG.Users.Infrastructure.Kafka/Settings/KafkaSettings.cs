using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.Infrastructure.Kafka.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class KafkaSettings
    {
        public string BootstrapServers { get; init; } = string.Empty;
        public string UserCreatedTopic { get; init; } = string.Empty;
    }
}
