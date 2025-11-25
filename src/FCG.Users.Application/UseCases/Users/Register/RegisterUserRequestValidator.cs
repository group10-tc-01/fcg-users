using FCG.Users.Domain.Users.ValueObjects;
using FCG.Users.Messages;
using FluentValidation;

namespace FCG.Users.Application.UseCases.Users.Register
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ResourceMessages.NameRequired)
                .MaximumLength(255)
                .WithMessage(ResourceMessages.NameCannotExceed255Characters);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ResourceMessages.LoginEmalRequired)
                .EmailAddress()
                .WithMessage(ResourceMessages.LoginInvalidEmailFormat);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ResourceMessages.LoginPasswordRequired)
                .MaximumLength(100)
                .WithMessage(ResourceMessages.LongPassword)
                .Must(BeValidPassword)
                .WithMessage(ResourceMessages.PasswordFormatNotValid);
        }

        private static bool BeValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                Password.Create(password);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
