using Confluent.Kafka;
using FCG.Users.Infrastructure.Kafka.Abstractions;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FCG.Users.Infrastructure.Kafka.Producer
{
    [ExcludeFromCodeCoverage]
    public class KafkaProducer : IKafkaProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private bool _disposed;

        public KafkaProducer(ProducerConfig config)
        {
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
        {

            var serializedMessage = JsonSerializer.Serialize(message);

            var kafkaMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = serializedMessage
            };

            try
            {
                var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

                if (deliveryResult.Status != PersistenceStatus.Persisted)
                {
                    throw new InvalidOperationException($"Failed to produce message to topic '{topic}'. Status: {deliveryResult.Status}");
                }
            }
            catch (ProduceException<string, string> ex)
            {
                throw new InvalidOperationException($"Error producing message to Kafka topic '{topic}': {ex.Error.Reason}", ex);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
