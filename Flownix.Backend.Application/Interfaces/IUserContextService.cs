using System.Security.Claims;

namespace Flownix.Backend.Application.Interfaces
{
    public interface IUserContextService
    {
        Guid? GetCurrentUserId();
        ClaimsPrincipal? GetCurrentUser();
        bool IsAuthenticated();
    }
}