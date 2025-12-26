using Flownix.Backend.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Flownix.Backend.API.Common
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator _mediator;
        private IUserContextService _userContextService;

        protected IMediator Mediator =>
            _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected IUserContextService UserContextService =>
            _userContextService ??= HttpContext.RequestServices.GetService<IUserContextService>();

        protected Guid UserId => UserContextService.GetCurrentUserId() ?? Guid.Empty;
    }
}