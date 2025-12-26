using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Contracts.DTOs.UpdateDTOs
{
    public class UpdateSensorDto
    {
        public SensorType? Type { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public string? Unit { get; set; }
        public bool? IsActive { get; set; }
    }
}