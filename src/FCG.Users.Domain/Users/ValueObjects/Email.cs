using FCG.Users.Domain.Exceptions;
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
                throw new DomainException("Email cannot be null or empty.");

            if (value.Length > 255)
                throw new DomainException("Email cannot exceed 255 characters.");

            if (!IsValidEmail(value))
                throw new DomainException("Invalid email format.");

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
