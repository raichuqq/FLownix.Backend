using Flownix.Backend.Contracts.DTOs.CreateDTOs;

namespace Flownix.Backend.Contracts.DTOs.AuthDTOs
{
    public class RegisterDto
    {
        public UserCreateDto User { get; set; } = null!;
    }
}
