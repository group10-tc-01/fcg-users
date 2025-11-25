using FCG.Users.Domain.Exceptions;

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
                throw new DomainException("Password cannot be null or empty.");

            if (value.Length < 8)
                throw new DomainException("Password must be at least 8 characters long.");

            if (!ContainsLetter(value))
                throw new DomainException("Password must contain at least one letter.");

            if (!ContainsDigit(value))
                throw new DomainException("Password must contain at least one digit.");

            if (!ContainsSpecialCharacter(value))
                throw new DomainException("Password must contain at least one special character.");

            return new Password(value);
        }

        public static Password CreateFromHash(string hashValue)
        {
            if (string.IsNullOrWhiteSpace(hashValue))
                throw new ArgumentNullException(nameof(hashValue), "Stored hash cannot be null or empty.");

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
