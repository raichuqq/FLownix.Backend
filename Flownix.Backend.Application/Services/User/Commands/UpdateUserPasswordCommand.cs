using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.User.Commands
{
    public class UpdateUserPasswordCommand : IRequest
    {
        public UserPasswordUpdateDto UserPassword { get; set; }

        public UpdateUserPasswordCommand(UserPasswordUpdateDto userPassword)
        {
            UserPassword = userPassword;
        }
    }

    public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand>
    {
        private readonly IFlownixDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IPasswordHasher _passwordHasher;
        public UpdateUserPasswordCommandHandler(IFlownixDbContext context,
            IUserContextService userContextService,
            IPasswordHasher passwordHasher)
        {
            _context = context;
            _userContextService = userContextService;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
            {
                throw new Exception("User is not authenticated");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);
            if (user == null)
            {
                throw new Exception($"TROUBLES: User with Id {userId} was not found in database");
            }

            var dto = request.UserPassword;

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new Exception("Current and new password must be provided");
            }

            user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}