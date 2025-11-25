using FCG.Users.Domain.Users.Events;
using FCG.Users.Infrastructure.Kafka.Abstractions;
using FCG.Users.Infrastructure.Kafka.Configuration;
using FCG.Users.Infrastructure.Kafka.Messages;
using MediatR;

namespace FCG.Users.Infrastructure.Kafka.EventsHandlers
{
    public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
    {
        private readonly IKafkaProducer _kafkaProducer;
        private readonly KafkaSettings _kafkaSettings;

        public UserCreatedDomainEventHandler(IKafkaProducer kafkaProducer, KafkaSettings kafkaSettings)
        {
            _kafkaProducer = kafkaProducer;
            _kafkaSettings = kafkaSettings;
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var message = new UserCreatedMessage
            {
                UserId = notification.UserId,
                Name = notification.Name,
                Email = notification.Email,
                CorrelationId = Guid.NewGuid(),
                OccurredAt = DateTime.UtcNow
            };

            await _kafkaProducer.ProduceAsync(_kafkaSettings.UserCreatedTopic, message, cancellationToken);
        }
    }
}
