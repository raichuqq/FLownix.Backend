using Flownix.Backend.Application.Services.Auth.Register;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using FluentValidation;
namespace Flownix.Backend.Application.Common.Validator.AuthRegister
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Register.User.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email")
                .MaximumLength(100).WithMessage("Email must not be longer than 100 symbols");

            When(x => !string.IsNullOrWhiteSpace(x.Register.User.PhoneNumber), () =>
            {
                RuleFor(x => x.Register.User.PhoneNumber)
                    .Matches(@"^\+380\d{9}$")
                    .WithMessage("Invalid phone number format. Please, write ukrainian phone number with +380");
            });

            RuleFor(x => x.Register.User.PasswordHash)
                .MinimumLength(8).WithMessage("Password's length must be at least 8 symbols")
                .MaximumLength(32).WithMessage("Password's length mustn't be longer than 32 symbols")
                .NotEmpty();

            RuleFor(x => x.Register.User.FirstName)
                .NotEmpty().MinimumLength(1).MaximumLength(50)
                .Matches(@"^[A-Za-z\s]+$").WithMessage("First name must have only letters and spaces");

            RuleFor(x => x.Register.User.LastName)
                .NotEmpty().MinimumLength(1).MaximumLength(50)
                .Matches(@"^[A-Za-z\s]+$").WithMessage("Last name must have only letters and spaces");
        }
    }
}