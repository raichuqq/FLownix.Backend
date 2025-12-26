using AutoMapper;
using AutoMapper.QueryableExtensions;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Alerts.Queries
{
    public class GetAllAlertsQuery : IRequest<List<AlertDto>>
    {
        public Guid? WaterObjectId { get; set; }
        public bool? IsRead { get; set; }
        public AlertType? Type { get; set; }
        public int? Take { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IncludeRead { get; set; } = true;
    }

    public class GetAllAlertsQueryHandler : IRequestHandler<GetAllAlertsQuery, List<AlertDto>>
    {
        private readonly IFlownixDbContext _context;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public GetAllAlertsQueryHandler(
            IFlownixDbContext context,
            IUserContextService userContextService,
            IMapper mapper)
        {
            _context = context;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<List<AlertDto>> Handle(GetAllAlertsQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var accessibleWaterObjectIds = await GetAccessibleWaterObjectIds(userId.Value, cancellationToken);

            if (!accessibleWaterObjectIds.Any())
            {
                return new List<AlertDto>();
            }

            var query = _context.Alerts
                .Include(a => a.WaterObject)
                .Include(a => a.Sensor)
                .Where(a => accessibleWaterObjectIds.Contains(a.WaterObjectId));

            if (request.WaterObjectId.HasValue)
            {
                if (!accessibleWaterObjectIds.Contains(request.WaterObjectId.Value))
                {
                    return new List<AlertDto>();
                }
                query = query.Where(a => a.WaterObjectId == request.WaterObjectId.Value);
            }

            if (request.IsRead.HasValue)
            {
                query = query.Where(a => a.IsRead == request.IsRead.Value);
            }
            else if (!request.IncludeRead)
            {
                query = query.Where(a => !a.IsRead);
            }

            if (request.Type.HasValue)
            {
                query = query.Where(a => a.Type == request.Type.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt <= request.ToDate.Value);
            }

            query = query.OrderBy(a => a.IsRead)
                        .ThenByDescending(a => a.CreatedAt);

            if (request.Take.HasValue && request.Take.Value > 0)
            {
                query = query.Take(request.Take.Value);
            }

            return await query
                .AsNoTracking()
                .ProjectTo<AlertDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }

        private async Task<List<Guid>> GetAccessibleWaterObjectIds(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user?.Role == Domain.Enums.Role.Admin)
            {
                return await _context.WaterObjects
                    .Select(wo => wo.Id)
                    .ToListAsync(cancellationToken);
            }

            return await _context.UserObjectAccesses
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.WaterObjectId)
                .ToListAsync(cancellationToken);
        }
    }
}