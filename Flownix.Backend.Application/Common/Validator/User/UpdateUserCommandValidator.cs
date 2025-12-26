using FluentValidation;
using Flownix.Backend.Application.Services.User.Commands;

namespace Flownix.Backend.Application.Common.Validator.User
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.User.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email")
                .MaximumLength(100).WithMessage("Email must not be longer than 100 symbols");

            When(x => !string.IsNullOrWhiteSpace(x.User.PhoneNumber), () =>
            {
                RuleFor(x => x.User.PhoneNumber)
                    .Matches(@"^\+380\d{9}$")
                    .WithMessage("Invalid phone number format. Please, write ukrainian phone number with +380");
            });

            RuleFor(x => x.User.FirstName)
                .NotEmpty().MinimumLength(1).MaximumLength(50)
                .Matches(@"^[A-Za-z\s]+$").WithMessage("First name must have only letters and spaces");

            RuleFor(x => x.User.LastName)
                .NotEmpty().MinimumLength(1).MaximumLength(50)
                .Matches(@"^[A-Za-z\s]+$").WithMessage("Last name must have only letters and spaces");
        }
    }
}