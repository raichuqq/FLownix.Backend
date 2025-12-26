using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using MediatR;

namespace Flownix.Backend.Application.Services.Pumps.Commands
{
    public class CreatePumpCommand : IRequest<Guid>
    {
        public required CreatePumpDto Pump { get; set; }
    }

    public class CreatePumpCommandHandler
        : IRequestHandler<CreatePumpCommand, Guid>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public CreatePumpCommandHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(
            CreatePumpCommand request,
            CancellationToken cancellationToken)
        {
            var pump = _mapper.Map<Domain.Entities.Pump>(request.Pump);

            pump.CreatedAt = DateTime.UtcNow;

            _context.Pumps.Add(pump);
            await _context.SaveChangesAsync(cancellationToken);

            return pump.Id;
        }
    }
}
