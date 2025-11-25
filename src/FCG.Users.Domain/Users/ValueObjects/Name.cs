using FCG.Users.Domain.Exceptions;
using FCG.Users.Messages;

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
                throw new DomainException(ResourceMessages.NameCannotBeNullOrEmpty);

            if (value.Length < 2)
                throw new DomainException(ResourceMessages.NameMinimumLength);

            if (value.Length > 255)
                throw new DomainException(ResourceMessages.NameCannotExceed255Characters);

            return new Name(value);
        }

        public static implicit operator Name(string value) => Create(value);
        public static implicit operator string(Name name) => name.Value;

        public override string ToString() => Value;
    }
}
