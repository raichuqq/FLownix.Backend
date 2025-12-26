namespace Flownix.Backend.Contracts.DTOs.UpdateDTOs
{
    public class UserPasswordUpdateDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}