using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Sensor.Queries
{
    public class GetMySensorsQuery : IRequest<List<SensorDto>>
    {
       
    }

    public class GetMySensorsQueryHandler
        : IRequestHandler<GetMySensorsQuery, List<SensorDto>>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContext;

        public GetMySensorsQueryHandler(
            IFlownixDbContext context,
            IMapper mapper,
            IUserContextService userContext)
        {
            _context = context;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<List<SensorDto>> Handle(
            GetMySensorsQuery request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _userContext.GetCurrentUserId();

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken);

            if (currentUser?.Role == Domain.Enums.Role.Admin)
            {
                var allSensors = await _context.Sensors.ToListAsync(cancellationToken);
                return _mapper.Map<List<SensorDto>>(allSensors);
            }

            var waterObjectIds = await _context.UserObjectAccesses
                .Where(ua => ua.UserId == currentUserId)
                .Select(ua => ua.WaterObjectId)
                .ToListAsync(cancellationToken);

            var sensors = await _context.Sensors
                .Where(s => waterObjectIds.Contains(s.WaterObjectId))
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<SensorDto>>(sensors);
        }
    }
}