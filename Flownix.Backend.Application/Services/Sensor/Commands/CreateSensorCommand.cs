using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;

namespace Flownix.Backend.Application.Services.Sensor.Commands
{
    public class CreateSensorCommand : IRequest<SensorDto>
    {
        public required CreateSensorDto Sensor { get; set; }
    }

    public class CreateSensorCommandHandler
        : IRequestHandler<CreateSensorCommand, SensorDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public CreateSensorCommandHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SensorDto> Handle(
            CreateSensorCommand request,
            CancellationToken cancellationToken)
        {
            var sensor = _mapper.Map<Domain.Entities.Sensor>(request.Sensor);

            sensor.CreatedAt = DateTime.UtcNow;
            sensor.IsActive = true; 

            _context.Sensors.Add(sensor);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SensorDto>(sensor);
        }
    }
}