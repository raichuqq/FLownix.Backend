using AutoMapper;
using AutoMapper.QueryableExtensions;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.Application.Services.SensorReading.Queries
{
    public class GetAllSensorReadingsQuery : IRequest<List<SensorReadingDto>>
    {
        public Guid? SensorId { get; set; }
        public int? Take { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public GetAllSensorReadingsQuery(Guid? sensorId = null, int? take = null,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            SensorId = sensorId;
            Take = take;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }

    public class GetAllSensorReadingsQueryHandler
        : IRequestHandler<GetAllSensorReadingsQuery, List<SensorReadingDto>>
    {
        private readonly IFlownixDbContext _context;
        private readonly IMapper _mapper;

        public GetAllSensorReadingsQueryHandler(
            IFlownixDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<SensorReadingDto>> Handle(
            GetAllSensorReadingsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.SensorReadings.AsNoTracking();

            if (request.SensorId.HasValue)
            {
                query = query.Where(r => r.SensorId == request.SensorId.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(r => r.RecordedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(r => r.RecordedAt <= request.ToDate.Value);
            }

            query = query.OrderByDescending(r => r.RecordedAt);

            if (request.Take.HasValue && request.Take.Value > 0)
            {
                query = query.Take(request.Take.Value);
            }

            return await query
                .ProjectTo<SensorReadingDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }
}