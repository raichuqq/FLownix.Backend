using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.UserObjectAccess.Queries
{
    public class GetAccessesByWaterObjectIdQuery : IRequest<List<UserObjectAccessDto>>
    {
        public Guid WaterObjectId { get; set; }
        public bool IncludeDetails { get; set; } = false;
    }

    public class GetAccessesByWaterObjectIdQueryHandler
        : IRequestHandler<GetAccessesByWaterObjectIdQuery, List<UserObjectAccessDto>>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetAccessesByWaterObjectIdQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserObjectAccessDto>> Handle(
            GetAccessesByWaterObjectIdQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.UserObjectAccesses
                .AsNoTracking()
                .Where(ua => ua.WaterObjectId == request.WaterObjectId);

            if (request.IncludeDetails)
            {
                query = query
                    .Include(ua => ua.User)
                    .Include(ua => ua.WaterObject);
            }

            var accesses = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<UserObjectAccessDto>>(accesses);
        }
    }
}