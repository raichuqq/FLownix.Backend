using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Sensor.Queries
{
    public class GetSensorByIdQuery : IRequest<SensorDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetSensorByIdQueryHandler
        : IRequestHandler<GetSensorByIdQuery, SensorDto?>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetSensorByIdQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SensorDto?> Handle(
            GetSensorByIdQuery request,
            CancellationToken cancellationToken)
        {
            var sensor = await _context.Sensors
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            return sensor == null
                ? null
                : _mapper.Map<SensorDto>(sensor);
        }
    }
}