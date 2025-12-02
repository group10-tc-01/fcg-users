using FCG.Users.Messages;
using FluentValidation;

namespace FCG.Users.Application.UseCases.Admin.GetUsers
{
    public class GetUsersRequestValidator : AbstractValidator<GetUsersRequest>
    {
        public GetUsersRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Name))
                .WithMessage(ResourceMessages.NameCannotExceed255Characters);

            RuleFor(x => x.Email)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage(ResourceMessages.EmailCannotExceed255Characters)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage(ResourceMessages.InvalidEmailFormat);
        }
    }
}
