using FluentValidation;

namespace FCG.Users.Application.UseCases.Admin.DeactivateUser
{
    public class DeactivateUserRequestValidator : AbstractValidator<DeactivateUserRequest>
    {
        public DeactivateUserRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}
