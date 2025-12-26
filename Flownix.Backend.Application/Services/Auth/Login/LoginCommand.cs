using Flownix.Backend.Contracts.DTOs.AuthDTOs;
using Flownix.Backend.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace Flownix.Backend.Application.Services.Auth.Login
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public LoginDto LoginDto { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IFlownixDbContext _context;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IFlownixDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.LoginDto.Email, cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            if (!VerifyPassword(request.LoginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password");

            // generate tokens
            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // update user's refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            user.LastLoginAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Role = (Contracts.DTOs.Enums.Role)user.Role
                }
            };
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            Console.WriteLine("-------- PASSWORD DEBUG --------");
            Console.WriteLine("RAW PASSWORD = '" + password + "'");
            Console.WriteLine("HASH FROM DB = '" + passwordHash + "'");
            Console.WriteLine("BCrypt.Verify = " + BCrypt.Net.BCrypt.Verify(password, passwordHash));
            Console.WriteLine("--------------------------------");

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}