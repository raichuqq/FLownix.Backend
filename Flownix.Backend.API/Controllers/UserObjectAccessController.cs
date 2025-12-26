using Flownix.Backend.API.Common;
using Flownix.Backend.Application.Interfaces;
using Flownix.Backend.Application.Services.UserObjectAccess.Commands;
using Flownix.Backend.Application.Services.UserObjectAccess.Queries;
using Flownix.Backend.Contracts.DTOs.CreateDTOs;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flownix.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserObjectAccessController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IUserContextService _userContext; 

        public UserObjectAccessController(
            IMediator mediator,
            IUserContextService userContext) 
        {
            _mediator = mediator;
            _userContext = userContext;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserObjectAccessDto>> GiveAccess(
            [FromBody] UserObjectAccessCreateDto dto)
        {
            var command = new GiveAccessCommand { Access = dto };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{userId}/{waterObjectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RevokeAccess(Guid userId, Guid waterObjectId)
        {
            var command = new RevokeAccessCommand
            {
                UserId = userId,
                WaterObjectId = waterObjectId
            };
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<UserObjectAccessDto>>> GetByUserId(
            Guid userId,
            [FromQuery] bool includeDetails = false)
        {
            var query = new GetAccessesByUserIdQuery
            {
                UserId = userId,
                IncludeDetails = includeDetails
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("waterobject/{waterObjectId}")]
        public async Task<ActionResult<List<UserObjectAccessDto>>> GetByWaterObjectId(
            Guid waterObjectId,
            [FromQuery] bool includeDetails = false)
        {
            var query = new GetAccessesByWaterObjectIdQuery
            {
                WaterObjectId = waterObjectId,
                IncludeDetails = includeDetails
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<List<UserObjectAccessDto>>> GetMyAccesses(
            [FromQuery] bool includeDetails = false)
        {
         
            var currentUserId = _userContext.GetCurrentUserId();

            if (currentUserId == null)
            {
                return Unauthorized("User not authenticated");
            }

            var query = new GetAccessesByUserIdQuery
            {
                UserId = currentUserId.Value, 
                IncludeDetails = includeDetails
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}