using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.User.Commands
{
    public class UpdateUserCommand : IRequest
    {
        public UserUpdateDto User { get; set; }
        public UpdateUserCommand(UserUpdateDto user)
        {
            User = user;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IFlownixDbContext _context;
        private readonly IUserContextService _userContextService;

        public UpdateUserCommandHandler(IFlownixDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContextService = userContextService;
        }

        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new Exception($"TROUBLE: User with id {userId} is not in database");
            }

            var dto = request.User;

            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                user.FirstName = dto.FirstName;
            }
            if (!string.IsNullOrEmpty(dto.LastName))
            {
                user.LastName = dto.LastName;
            }
            if (!string.IsNullOrEmpty(dto.Email))
            {
                user.Email = dto.Email;
            }
            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                user.PhoneNumber = dto.PhoneNumber;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}