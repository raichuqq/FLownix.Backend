using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Contracts.DTOs.ReadingDTOs
{
    public class SensorDto
    {
        public Guid Id { get; set; }
        public Guid WaterObjectId { get; set; }
        public SensorType Type { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public string Unit { get; set; } = null!;
        public bool IsActive { get; set; }

        
    }
}