using FCG.Users.Application.Abstractions.Messaging;
using FCG.Users.Domain.Users.Events;
using FCG.Users.Infrastructure.Kafka.Messages;
using FCG.Users.Infrastructure.Kafka.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.Infrastructure.Kafka.EventsHandlers
{
    [ExcludeFromCodeCoverage]
    public class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
    {
        private readonly IMessageProducer _messageProducer;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserCreatedDomainEventHandler> _logger;

        public UserCreatedDomainEventHandler(
            IMessageProducer kafkaProducer,
            KafkaSettings kafkaSettings,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserCreatedDomainEventHandler> logger)
        {
            _messageProducer = kafkaProducer;
            _kafkaSettings = kafkaSettings;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            var correlationId = GetCorrelationId();

            _logger.LogInformation(
                "Processing UserCreatedDomainEvent for UserId {UserId} with CorrelationId {CorrelationId}",
                notification.UserId, correlationId);

            var message = new UserCreatedMessage
            {
                UserId = notification.UserId,
                Name = notification.Name,
                Email = notification.Email,
                CorrelationId = correlationId,
                OccurredAt = DateTime.UtcNow
            };

            _logger.LogDebug(
                "Created UserCreatedMessage for UserId {UserId}, Email {Email}, Topic {Topic}, CorrelationId {CorrelationId}",
                message.UserId, message.Email, _kafkaSettings.UserCreatedTopic, correlationId);

            try
            {
                await _messageProducer.ProduceAsync(_kafkaSettings.UserCreatedTopic, message, cancellationToken);

                _logger.LogInformation(
                    "Successfully completed UserCreatedDomainEvent processing for UserId {UserId} with CorrelationId {CorrelationId}",
                    notification.UserId, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to process UserCreatedDomainEvent for UserId {UserId} with CorrelationId {CorrelationId}",
                    notification.UserId, correlationId);
                throw;
            }
        }

        private Guid GetCorrelationId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.Items.TryGetValue("CorrelationId", out var correlationIdObj) == true
                && correlationIdObj is string correlationIdString
                && Guid.TryParse(correlationIdString, out var correlationId))
            {
                return correlationId;
            }

            return Guid.NewGuid();
        }
    }
}
