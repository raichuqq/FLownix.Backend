using Flownix.Backend.API.Common;
using Flownix.Backend.Application.Services.Auth.Login;
using Flownix.Backend.Application.Services.Auth.Register;
using Flownix.Backend.Contracts.DTOs.AuthDTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flownix.Backend.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterDto register)
        {
            var command = new RegisterUserCommand { Register = register };
            var UserId = await Mediator.Send(command);
            return Ok(UserId);
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var command = new LoginCommand { LoginDto = loginDto };
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var command = new RefreshTokenCommand { RefreshTokenDto = refreshTokenDto };
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}