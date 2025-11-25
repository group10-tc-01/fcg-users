namespace FCG.Users.Infrastructure.Kafka.Abstractions
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class;
    }
}
