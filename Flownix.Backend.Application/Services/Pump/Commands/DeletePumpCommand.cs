using Flownix.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Pumps.Commands
{
    public class DeletePumpCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeletePumpCommandHandler
        : IRequestHandler<DeletePumpCommand>
    {
        private readonly IFlownixDbContext _context;

        public DeletePumpCommandHandler(IFlownixDbContext context)
        {
            _context = context;
        }

        public async Task Handle(
            DeletePumpCommand request,
            CancellationToken cancellationToken)
        {
            var pump = await _context.Pumps
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (pump == null)
                throw new Exception("Pump not found");

            _context.Pumps.Remove(pump);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
