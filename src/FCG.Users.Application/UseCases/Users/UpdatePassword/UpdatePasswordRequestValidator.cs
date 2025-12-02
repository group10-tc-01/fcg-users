using FCG.Users.Messages;
using FluentValidation;

namespace FCG.Users.Application.UseCases.Users.UpdatePassword
{
    public class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequest>
    {
        public UpdatePasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(ResourceMessages.LoginPasswordRequired);

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(ResourceMessages.CurrentPasswordRequired)
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));

            RuleFor(x => x)
                .Must(x => x.CurrentPassword != x.NewPassword)
                .WithMessage(ResourceMessages.NewPasswordMustBeDifferent)
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword) && !string.IsNullOrWhiteSpace(x.CurrentPassword));
        }
    }
}
