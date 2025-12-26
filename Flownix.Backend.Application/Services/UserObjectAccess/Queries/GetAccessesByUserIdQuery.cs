using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.UserObjectAccess.Queries
{
    public class GetAccessesByUserIdQuery : IRequest<List<UserObjectAccessDto>>
    {
        public Guid UserId { get; set; }
        public bool IncludeDetails { get; set; } = false; // Включать ли данные User/WaterObject
    }

    public class GetAccessesByUserIdQueryHandler
        : IRequestHandler<GetAccessesByUserIdQuery, List<UserObjectAccessDto>>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetAccessesByUserIdQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserObjectAccessDto>> Handle(
            GetAccessesByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.UserObjectAccesses
                .AsNoTracking()
                .Where(ua => ua.UserId == request.UserId);

            // Если нужны детали
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