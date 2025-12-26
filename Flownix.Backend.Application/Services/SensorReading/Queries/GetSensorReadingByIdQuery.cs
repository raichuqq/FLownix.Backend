using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.SensorReading.Queries
{
    public class GetSensorReadingByIdQuery : IRequest<SensorReadingDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetSensorReadingByIdQueryHandler
        : IRequestHandler<GetSensorReadingByIdQuery, SensorReadingDto?>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetSensorReadingByIdQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SensorReadingDto?> Handle(
            GetSensorReadingByIdQuery request,
            CancellationToken cancellationToken)
        {
            var reading = await _context.SensorReadings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            return reading == null
                ? null
                : _mapper.Map<SensorReadingDto>(reading);
        }
    }
}