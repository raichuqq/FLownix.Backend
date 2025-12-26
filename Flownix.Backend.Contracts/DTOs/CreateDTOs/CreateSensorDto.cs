using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Contracts.DTOs.CreateDTOs
{
    public class CreateSensorDto
    {
        public required Guid WaterObjectId { get; set; }
        public required SensorType Type { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public required string Unit { get; set; }
        public bool IsActive { get; set; } = true;
    }
}