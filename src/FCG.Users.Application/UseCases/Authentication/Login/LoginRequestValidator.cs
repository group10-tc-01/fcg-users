using FCG.Users.Domain.Users.ValueObjects;
using FCG.Users.Messages;
using FluentValidation;

namespace FCG.Users.Application.UseCases.Authentication.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ResourceMessages.LoginEmalRequired)
                .Must(BeValidEmail)
                .WithMessage(ResourceMessages.LoginInvalidEmailFormat);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ResourceMessages.LoginPasswordRequired);
        }

        private static bool BeValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                Email.Create(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
