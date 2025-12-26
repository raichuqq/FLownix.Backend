using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Pumps.Queries
{
    public class GetPumpByIdQuery : IRequest<PumpDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetPumpByIdQueryHandler
        : IRequestHandler<GetPumpByIdQuery, PumpDto?>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetPumpByIdQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PumpDto?> Handle(
            GetPumpByIdQuery request,
            CancellationToken cancellationToken)
        {
            var pump = await _context.Pumps
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            return pump == null
                ? null
                : _mapper.Map<PumpDto>(pump);
        }
    }
}
