using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Contracts.DTOs.CreateDTOs
{
    public class UserCreateDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Role Role { get; set; }
        public required string PasswordHash { get; set; }
    }
}