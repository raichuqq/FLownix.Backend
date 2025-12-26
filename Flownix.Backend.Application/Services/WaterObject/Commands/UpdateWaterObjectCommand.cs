using AutoMapper;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.WaterObject.Commands
{
    public class UpdateWaterObjectCommand : IRequest<WaterObjectDto>
    {
        public Guid Id { get; set; }
        public required UpdateWaterObjectDto WaterObject { get; set; }
    }

    public class UpdateWaterObjectCommandHandler
        : IRequestHandler<UpdateWaterObjectCommand, WaterObjectDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public UpdateWaterObjectCommandHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WaterObjectDto> Handle(
            UpdateWaterObjectCommand request,
            CancellationToken cancellationToken)
        {
            var waterObject = await _context.WaterObjects
                .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

            if (waterObject == null)
                throw new Exception("Water object not found");

            _mapper.Map(request.WaterObject, waterObject);
            waterObject.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<WaterObjectDto>(waterObject);
        }
    }
}