using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Application.Services.Sensor.Queries
{
    public class GetAllSensorsQuery : IRequest<List<SensorDto>>
    {
        public Guid? WaterObjectId { get; set; }
        public bool? IsActive { get; set; }
        public SensorType? Type { get; set; }
    }

    public class GetAllSensorsQueryHandler
        : IRequestHandler<GetAllSensorsQuery, List<SensorDto>>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetAllSensorsQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SensorDto>> Handle(
            GetAllSensorsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Sensors.AsNoTracking();

            // Фильтры
            if (request.WaterObjectId.HasValue)
            {
                query = query.Where(s => s.WaterObjectId == request.WaterObjectId.Value);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            if (request.Type.HasValue)
            {
                var domainType = (Domain.Enums.SensorType)request.Type.Value;
                query = query.Where(s => s.Type == domainType);
            }

            var sensors = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<SensorDto>>(sensors);
        }
    }
}