using FCG.Users.Domain.Users;
using FCG.Users.Domain.Users.ValueObjects;
using FCG.Users.Messages;
using FluentValidation;

namespace FCG.Users.Application.UseCases.Admin.CreateUser
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
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

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role specified.");
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
