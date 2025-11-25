using FCG.Users.Domain.Exceptions;

namespace FCG.Users.Domain.Users.ValueObjects
{
    public record Name
    {
        public string Value { get; }

        private Name(string value)
        {
            Value = value;
        }

        public static Name Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Name cannot be null or empty.");

            if (value.Length < 2)
                throw new DomainException("Name must be at least 2 characters long.");

            if (value.Length > 255)
                throw new DomainException("Name cannot exceed 255 characters.");

            return new Name(value);
        }

        public static implicit operator Name(string value) => Create(value);
        public static implicit operator string(Name name) => name.Value;

        public override string ToString() => Value;
    }
}
