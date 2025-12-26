using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Alerts.Queries
{
    public class GetAlertByIdQuery : IRequest<AlertDto>
    {
        public Guid Id { get; }

        public GetAlertByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetAlertByIdQueryHandler : IRequestHandler<GetAlertByIdQuery, AlertDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public GetAlertByIdQueryHandler(
            IFlownixDbContext context,
            IUserContextService userContextService,
            IMapper mapper)
        {
            _context = context;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<AlertDto> Handle(GetAlertByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var alert = await _context.Alerts
                .Include(a => a.WaterObject)
                .Include(a => a.Sensor)
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (alert == null)
            {
                throw new Exception($"Alert with id {request.Id} was not found.");
            }

            var hasAccess = await HasAccessToWaterObject(userId.Value, alert.WaterObjectId, cancellationToken);
            if (!hasAccess)
            {
                throw new UnauthorizedAccessException("No access to this alert");
            }

            if (!alert.IsRead)
            {
                alert.IsRead = true;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return _mapper.Map<AlertDto>(alert);
        }

        private async Task<bool> HasAccessToWaterObject(Guid userId, Guid waterObjectId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user?.Role == Domain.Enums.Role.Admin)
            {
                return true;
            }

            return await _context.UserObjectAccesses
                .AnyAsync(ua => ua.UserId == userId && ua.WaterObjectId == waterObjectId, cancellationToken);
        }
    }
}