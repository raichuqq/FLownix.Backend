using Flownix.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.UserObjectAccess.Commands
{
    public class RevokeAccessCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid WaterObjectId { get; set; }
    }

    public class RevokeAccessCommandHandler : IRequestHandler<RevokeAccessCommand>
    {
        private readonly IFlownixDbContext _context;

        public RevokeAccessCommandHandler(IFlownixDbContext context)
        {
            _context = context;
        }

        public async Task Handle(
            RevokeAccessCommand request,
            CancellationToken cancellationToken)
        {
            var access = await _context.UserObjectAccesses
                .FirstOrDefaultAsync(ua =>
                    ua.UserId == request.UserId &&
                    ua.WaterObjectId == request.WaterObjectId,
                    cancellationToken);

            if (access == null)
            {
                return; // Доступа і так нема   
            }

            _context.UserObjectAccesses.Remove(access);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}