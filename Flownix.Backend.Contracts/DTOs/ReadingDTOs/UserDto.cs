using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Contracts.DTOs.ReadingDTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Role Role { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}