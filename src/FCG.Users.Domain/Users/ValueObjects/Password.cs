using FCG.Users.Domain.Exceptions;
using FCG.Users.Messages;

namespace FCG.Users.Domain.Users.ValueObjects
{
    public record Password
    {
        public string Value { get; }

        private Password(string value)
        {
            Value = value;
        }

        public static Password Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(ResourceMessages.PasswordCannotBeNullOrEmpty);

            if (value.Length < 8)
                throw new DomainException(ResourceMessages.PasswordMinimumLength);

            if (!ContainsLetter(value))
                throw new DomainException(ResourceMessages.PasswordMustContainLetter);

            if (!ContainsDigit(value))
                throw new DomainException(ResourceMessages.PasswordMustContainNumber);

            if (!ContainsSpecialCharacter(value))
                throw new DomainException(ResourceMessages.PasswordMustContainSpecialCharacter);

            return new Password(value);
        }

        public static Password CreateFromHash(string hashValue)
        {
            if (string.IsNullOrWhiteSpace(hashValue))
                throw new ArgumentNullException(nameof(hashValue), ResourceMessages.PasswordCannotBeNullOrEmpty);

            return new Password(hashValue);
        }

        private static bool ContainsLetter(string password) => password.Any(char.IsLetter);

        private static bool ContainsDigit(string password) => password.Any(char.IsDigit);

        private static bool ContainsSpecialCharacter(string password) => password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

        public static implicit operator string(Password password) => password.Value;
        public static implicit operator Password(string value) => Create(value);

        public override string ToString() => Value;
    }
}
