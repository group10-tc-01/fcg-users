using FluentValidation;

namespace FCG.Users.Application.UseCases.Admin.UpdateUserRole
{
    public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
    {
        public UpdateUserRoleRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(x => x.NewRole)
                .IsInEnum()
                .WithMessage("Invalid role specified.");
        }
    }
}
