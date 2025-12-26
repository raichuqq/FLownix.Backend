using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Contracts.DTOs.AuthDTOs
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public Role Role { get; set; }
    }
}