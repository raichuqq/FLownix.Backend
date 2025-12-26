using System;

namespace Flownix.Backend.Contracts.DTOs.AuthDTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = null!;
    }
}
