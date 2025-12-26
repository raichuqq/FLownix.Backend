using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;

namespace Flownix.Backend.Application.Services.WaterObject.Commands
{
    public class CreateWaterObjectCommand : IRequest<WaterObjectDto>
    {
        public required CreateWaterObjectDto WaterObject { get; set; }
    }

    public class CreateWaterObjectCommandHandler
        : IRequestHandler<CreateWaterObjectCommand, WaterObjectDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public CreateWaterObjectCommandHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WaterObjectDto> Handle(
            CreateWaterObjectCommand request,
            CancellationToken cancellationToken)
        {
            var waterObject = _mapper.Map<Domain.Entities.WaterObject>(request.WaterObject);

            waterObject.CreatedAt = DateTime.UtcNow;

            _context.WaterObjects.Add(waterObject);
            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<WaterObjectDto>(waterObject);
        }
    }
}