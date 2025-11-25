namespace FCG.Users.Infrastructure.Kafka.Configuration
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string UserCreatedTopic { get; set; } = string.Empty;
    }
}
