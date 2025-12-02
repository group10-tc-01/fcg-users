using Confluent.Kafka;
using FCG.Users.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FCG.Users.Infrastructure.Kafka.Producer
{
    [ExcludeFromCodeCoverage]
    public class KafkaProducer : IMessageProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;
        private bool _disposed;

        public KafkaProducer(ProducerConfig config, ILogger<KafkaProducer> logger)
        {
            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger = logger;
        }

        public async Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
        {
            var correlationId = ExtractCorrelationId(message);
            var messageKey = Guid.NewGuid().ToString();

            _logger.LogInformation(
                "Starting Kafka message publication to topic {Topic} with Key {MessageKey} and CorrelationId {CorrelationId}",
                topic, messageKey, correlationId);

            var serializedMessage = JsonSerializer.Serialize(message);

            var kafkaMessage = new Message<string, string>
            {
                Key = messageKey,
                Value = serializedMessage
            };

            try
            {
                var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, cancellationToken);

                if (deliveryResult.Status != PersistenceStatus.Persisted)
                {
                    _logger.LogError(
                        "Failed to produce message to Kafka topic {Topic}. Status: {Status}, Key: {MessageKey}, CorrelationId: {CorrelationId}",
                        topic, deliveryResult.Status, messageKey, correlationId);

                    throw new InvalidOperationException($"Failed to produce message to topic '{topic}'. Status: {deliveryResult.Status}");
                }

                _logger.LogInformation(
                    "Successfully published message to Kafka topic {Topic}. Partition: {Partition}, Offset: {Offset}, Key: {MessageKey}, CorrelationId: {CorrelationId}",
                    topic, deliveryResult.Partition.Value, deliveryResult.Offset.Value, messageKey, correlationId);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex,
                    "Error producing message to Kafka topic {Topic}. Reason: {Reason}, Key: {MessageKey}, CorrelationId: {CorrelationId}",
                    topic, ex.Error.Reason, messageKey, correlationId);

                throw new InvalidOperationException($"Error producing message to Kafka topic '{topic}': {ex.Error.Reason}", ex);
            }
        }

        private static Guid ExtractCorrelationId<T>(T message) where T : class
        {
            var correlationIdProperty = typeof(T).GetProperty("CorrelationId");
            if (correlationIdProperty != null && correlationIdProperty.PropertyType == typeof(Guid))
            {
                var value = correlationIdProperty.GetValue(message);
                if (value is Guid correlationId && correlationId != Guid.Empty)
                {
                    return correlationId;
                }
            }

            return Guid.Empty;
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
