using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.SensorReading.Commands
{
    public class CreateSensorReadingCommand : IRequest<SensorReadingDto>
    {
        public required CreateSensorReadingDto Reading { get; set; }
    }

    public class CreateSensorReadingCommandHandler
        : IRequestHandler<CreateSensorReadingCommand, SensorReadingDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAlertService _alertService; 

        public CreateSensorReadingCommandHandler(
            IFlownixDbContext context,
            IMapper mapper,
            IAlertService alertService) 
        {
            _context = context;
            _mapper = mapper;
            _alertService = alertService; 
        }

        public async Task<SensorReadingDto> Handle(
            CreateSensorReadingCommand request,
            CancellationToken cancellationToken)
        {
            var sensorExists = await _context.Sensors
                .AnyAsync(s => s.Id == request.Reading.SensorId, cancellationToken);

            if (!sensorExists)
            {
                throw new Exception("Sensor not found");
            }

            var reading = _mapper.Map<Domain.Entities.SensorReading>(request.Reading);
            reading.RecordedAt = DateTime.UtcNow;
            reading.CreatedAt = DateTime.UtcNow;

            _context.SensorReadings.Add(reading);
            await _context.SaveChangesAsync(cancellationToken);

            await _alertService.CheckAndCreateAlertsAsync(reading, cancellationToken);

            return _mapper.Map<SensorReadingDto>(reading);
        }
    }
}