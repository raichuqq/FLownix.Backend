namespace Flownix.Backend.Contracts.DTOs.CreateDTOs
{
    public class CreatePumpDto
    {
        public Guid WaterObjectId { get; set; }
        public string Name { get; set; } = null!;
    }
}