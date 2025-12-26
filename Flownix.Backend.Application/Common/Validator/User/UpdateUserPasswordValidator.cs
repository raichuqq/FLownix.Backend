using FluentValidation;
using Flownix.Backend.Application.Services.User.Commands;

namespace Flownix.Backend.Application.Common.Validator.User
{
    public class UpdateUserPasswordValidator : AbstractValidator<UpdateUserPasswordCommand>
    {
        public UpdateUserPasswordValidator()
        {
            RuleFor(x => x.UserPassword.CurrentPassword).NotEmpty();

            RuleFor(x => x.UserPassword.NewPassword)
                .MinimumLength(8).WithMessage("Password's length must be at least 8 symbols")
                .MaximumLength(32).WithMessage("Password's length mustn't be longer than 32 symbols")
                .NotEmpty();
        }
    }
}