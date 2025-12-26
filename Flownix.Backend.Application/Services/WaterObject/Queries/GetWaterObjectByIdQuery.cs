using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.WaterObject.Queries
{
    public class GetWaterObjectByIdQuery : IRequest<WaterObjectDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetWaterObjectByIdQueryHandler
        : IRequestHandler<GetWaterObjectByIdQuery, WaterObjectDto?>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetWaterObjectByIdQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WaterObjectDto?> Handle(
            GetWaterObjectByIdQuery request,
            CancellationToken cancellationToken)
        {
            var waterObject = await _context.WaterObjects
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            return waterObject == null
                ? null
                : _mapper.Map<WaterObjectDto>(waterObject);
        }
    }
}