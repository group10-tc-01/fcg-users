using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.Infrastructure.Kafka.Configuration
{
    [ExcludeFromCodeCoverage]
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string UserCreatedTopic { get; set; } = string.Empty;
    }
}
