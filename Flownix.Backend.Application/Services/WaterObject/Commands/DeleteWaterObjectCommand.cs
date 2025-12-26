using Flownix.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.WaterObject.Commands
{
    public class DeleteWaterObjectCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteWaterObjectCommandHandler
        : IRequestHandler<DeleteWaterObjectCommand>
    {
        private readonly IFlownixDbContext _context;

        public DeleteWaterObjectCommandHandler(IFlownixDbContext context)
        {
            _context = context;
        }

        public async Task Handle(
            DeleteWaterObjectCommand request,
            CancellationToken cancellationToken)
        {
            var waterObject = await _context.WaterObjects
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (waterObject == null)
                throw new Exception("Water object not found");

            _context.WaterObjects.Remove(waterObject);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}