using Flownix.Backend.API.Common;
using Flownix.Backend.Application.Services.Sensor.Commands;
using Flownix.Backend.Application.Services.Sensor.Queries;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flownix.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SensorsController : BaseController
    {
        private readonly IMediator _mediator;

        public SensorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/sensors
        [HttpGet]
        public async Task<ActionResult<List<SensorDto>>> GetAll(
            [FromQuery] Guid? waterObjectId,
            [FromQuery] bool? isActive,
            [FromQuery] Contracts.DTOs.Enums.SensorType? type)
        {
            var query = new GetAllSensorsQuery
            {
                WaterObjectId = waterObjectId,
                IsActive = isActive,
                Type = type
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/sensors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SensorDto>> GetById(Guid id)
        {
            var query = new GetSensorByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET: api/sensors/my
        [HttpGet("my")]
        public async Task<ActionResult<List<SensorDto>>> GetMy()
        {
            var query = new GetMySensorsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // POST: api/sensors
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SensorDto>> Create(CreateSensorDto dto)
        {
            var command = new CreateSensorCommand { Sensor = dto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/sensors/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SensorDto>> Update(Guid id, UpdateSensorDto dto)
        {
            var command = new UpdateSensorCommand
            {
                Id = id,
                Sensor = dto
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}