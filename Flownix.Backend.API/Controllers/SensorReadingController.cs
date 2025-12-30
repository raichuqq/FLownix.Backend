using Flownix.Backend.API.Common;
using Flownix.Backend.Application.Services.SensorReading.Commands;
using Flownix.Backend.Application.Services.SensorReading.Queries;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flownix.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorReadingController : BaseController
    {
        private readonly IMediator _mediator;

        public SensorReadingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/sensorreading
        [HttpGet]
        public async Task<ActionResult<List<SensorReadingDto>>> GetAll(
            [FromQuery] Guid? sensorId,
            [FromQuery] int? take,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var query = new GetAllSensorReadingsQuery
            {
                SensorId = sensorId,
                Take = take,
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/sensorreading/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SensorReadingDto>> GetById(Guid id)
        {
            var query = new GetSensorReadingByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/sensorreading
        [HttpPost]
        [AllowAnonymous] // ← ВАЖНО: Разрешаем анонимный доступ к этому методу
        public async Task<ActionResult<SensorReadingDto>> Create(CreateSensorReadingDto dto)
        {
            var command = new CreateSensorReadingCommand { Reading = dto };
            var result = await _mediator.Send(command);
            return Ok(result.Id);

        }
    }
}