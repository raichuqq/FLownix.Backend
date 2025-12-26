using Flownix.Backend.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Flownix.Backend.Infrastructure.Integration.Authentication
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier);

            return userIdClaim != null
                ? Guid.Parse(userIdClaim.Value)
                : null;
        }

        public ClaimsPrincipal? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?
                .User?
                .Identity?
                .IsAuthenticated ?? false;
        }
    }
}
