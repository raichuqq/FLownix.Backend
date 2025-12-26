using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.WaterObject.Queries
{
    public class GetAllWaterObjectsQuery : IRequest<List<WaterObjectDto>>
    {
        // Пока без фильтров, потом добавим
    }

    public class GetAllWaterObjectsQueryHandler
        : IRequestHandler<GetAllWaterObjectsQuery, List<WaterObjectDto>>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetAllWaterObjectsQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<WaterObjectDto>> Handle(
            GetAllWaterObjectsQuery request,
            CancellationToken cancellationToken)
        {
            var waterObjects = await _context.WaterObjects
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<WaterObjectDto>>(waterObjects);
        }
    }
}