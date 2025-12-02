using FCG.Users.Domain.Exceptions;
using FCG.Users.Messages;
using System.Net.Mail;

namespace FCG.Users.Domain.Users.ValueObjects
{
    public record Email
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(ResourceMessages.EmailCannotBeNullOrEmpty);

            if (value.Length > 255)
                throw new DomainException(ResourceMessages.EmailCannotExceed255Characters);

            if (!IsValidEmail(value))
                throw new DomainException(ResourceMessages.InvalidEmailFormat);

            return new Email(value);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return MailAddress.TryCreate(email, out _);
        }

        public static implicit operator string(Email email) => email.Value;
        public static implicit operator Email(string value) => Create(value);

        public override string ToString() => Value;
    }
}
