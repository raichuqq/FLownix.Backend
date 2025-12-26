using Flownix.Backend.Application.Services.Pumps.Commands;
using Flownix.Backend.Application.Services.Pumps.Queries;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using Flownix.Backend.Contracts.DTOs.UpdateDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Flownix.Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flownix.Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PumpsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFlownixDbContext _context;

        public PumpsController(
            IMediator mediator,
            IFlownixDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet]
        public async Task<ActionResult<List<PumpDto>>> GetAll(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var result = await _mediator.Send(
                new GetAllPumpsQuery(),
                cancellationToken);

            if (!isAdmin)
            {
                var accessibleObjectIds = await _context.UserObjectAccesses
                    .Where(ua => ua.UserId == userId)
                    .Select(ua => ua.WaterObjectId)
                    .ToListAsync(cancellationToken);

                result = result.Where(p => accessibleObjectIds.Contains(p.WaterObjectId)).ToList();
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PumpDto>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                var hasAccess = await _context.Pumps
                    .Where(p => p.Id == id)
                    .Select(p => p.WaterObject.UserAccesses.Any(ua => ua.UserId == userId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (!hasAccess)
                    return Forbid();
            }

            var result = await _mediator.Send(
                new GetPumpByIdQuery { Id = id },
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreatePumpDto dto,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(
                new CreatePumpCommand { Pump = dto },
                cancellationToken);

            return Ok(id);
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdatePumpDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                var hasAccess = await _context.Pumps
                    .Where(p => p.Id == id)
                    .Select(p => p.WaterObject.UserAccesses.Any(ua => ua.UserId == userId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (!hasAccess)
                    return Forbid();

                if (!string.IsNullOrEmpty(dto.Name))
                {
                    return BadRequest("Operators can only change pump status");
                }
            }

            await _mediator.Send(
                new UpdatePumpCommand
                {
                    Id = id,
                    Pump = dto
                },
                cancellationToken);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(
                new DeletePumpCommand { Id = id },
                cancellationToken);

            return NoContent();
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPost("{id:guid}/turn-on")]
        public async Task<IActionResult> TurnOn(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                var hasAccess = await _context.Pumps
                    .Where(p => p.Id == id)
                    .Select(p => p.WaterObject.UserAccesses.Any(ua => ua.UserId == userId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (!hasAccess)
                    return Forbid();
            }

            await _mediator.Send(
                new UpdatePumpCommand
                {
                    Id = id,
                    Pump = new UpdatePumpDto
                    {
                        Status = Contracts.DTOs.Enums.PumpStatus.On
                    }
                },
                cancellationToken);

            return NoContent();
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPost("{id:guid}/turn-off")]
        public async Task<IActionResult> TurnOff(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin)
            {
                var hasAccess = await _context.Pumps
                    .Where(p => p.Id == id)
                    .Select(p => p.WaterObject.UserAccesses.Any(ua => ua.UserId == userId))
                    .FirstOrDefaultAsync(cancellationToken);

                if (!hasAccess)
                    return Forbid();
            }

            await _mediator.Send(
                new UpdatePumpCommand
                {
                    Id = id,
                    Pump = new UpdatePumpDto
                    {
                        Status = Contracts.DTOs.Enums.PumpStatus.Off
                    }
                },
                cancellationToken);

            return NoContent();
        }

        #region Helper Methods

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        #endregion
    }
}