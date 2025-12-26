using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.Pumps.Queries
{
    public class GetAllPumpsQuery : IRequest<List<PumpDto>>
    {
    }

    public class GetAllPumpsQueryHandler
        : IRequestHandler<GetAllPumpsQuery, List<PumpDto>>
    {
        private readonly IFlownixDbContext _context;

        public GetAllPumpsQueryHandler(IFlownixDbContext context)
        {
            _context = context;
        }

        public async Task<List<PumpDto>> Handle(
            GetAllPumpsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Pumps
                .AsNoTracking()
                .Select(p => new PumpDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    WaterObjectId = p.WaterObjectId, 
                    Status = (Contracts.DTOs.Enums.PumpStatus)p.Status, 
                    LastUpdated = p.UpdatedAt 
                })
                .ToListAsync(cancellationToken);
        }
    }
}