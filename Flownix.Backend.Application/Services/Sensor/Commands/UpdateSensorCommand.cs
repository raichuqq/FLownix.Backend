using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Sensor.Commands
{
    public class UpdateSensorCommand : IRequest<SensorDto>
    {
        public Guid Id { get; set; }
        public required UpdateSensorDto Sensor { get; set; }
    }

    public class UpdateSensorCommandHandler
        : IRequestHandler<UpdateSensorCommand, SensorDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public UpdateSensorCommandHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SensorDto> Handle(
            UpdateSensorCommand request,
            CancellationToken cancellationToken)
        {
            var sensor = await _context.Sensors
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (sensor == null)
                throw new Exception("Sensor not found");

            UpdateSensorFields(sensor, request.Sensor);
            sensor.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SensorDto>(sensor);
        }

        private void UpdateSensorFields(Domain.Entities.Sensor sensor, UpdateSensorDto updateDto)
        {
            if (updateDto.Type.HasValue)
                sensor.Type = (Domain.Enums.SensorType)updateDto.Type.Value; 

            if (updateDto.MinValue.HasValue)
                sensor.MinValue = updateDto.MinValue.Value;

            if (updateDto.MaxValue.HasValue)
                sensor.MaxValue = updateDto.MaxValue.Value;

            if (!string.IsNullOrEmpty(updateDto.Unit))
                sensor.Unit = updateDto.Unit;

            if (updateDto.IsActive.HasValue)
                sensor.IsActive = updateDto.IsActive.Value;
        }
    }
}