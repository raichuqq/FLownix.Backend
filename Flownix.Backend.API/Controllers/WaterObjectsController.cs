using Flownix.Backend.API.Common;
using Flownix.Backend.Application.Services.WaterObject.Commands;
using Flownix.Backend.Application.Services.WaterObject.Queries;
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
    public class WaterObjectsController : BaseController
    {
        private readonly IMediator _mediator;

        public WaterObjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<WaterObjectDto>>> GetAll()
        {
            var query = new GetAllWaterObjectsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WaterObjectDto>> GetById(Guid id)
        {
            var query = new GetWaterObjectByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<WaterObjectDto>> Create(CreateWaterObjectDto dto)
        {
            var command = new CreateWaterObjectCommand { WaterObject = dto };
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WaterObjectDto>> Update(Guid id, UpdateWaterObjectDto dto)
        {
            var command = new UpdateWaterObjectCommand
            {
                Id = id,
                WaterObject = dto
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteWaterObjectCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}