using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Pumps.Commands
{
    public class UpdatePumpCommand : IRequest
    {
        public Guid Id { get; set; }
        public required UpdatePumpDto Pump { get; set; }
    }

    public class UpdatePumpCommandHandler
        : IRequestHandler<UpdatePumpCommand>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public UpdatePumpCommandHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(
            UpdatePumpCommand request,
            CancellationToken cancellationToken)
        {
            var pump = await _context.Pumps
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (pump == null)
                throw new Exception("Pump not found");

            if (!string.IsNullOrEmpty(request.Pump.Name))
            {
                pump.Name = request.Pump.Name;
            }

            if (request.Pump.Status.HasValue)
            {
                pump.Status = (Domain.Enums.PumpStatus)request.Pump.Status.Value;
                pump.LastUpdated = DateTime.UtcNow;
            }

            pump.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}