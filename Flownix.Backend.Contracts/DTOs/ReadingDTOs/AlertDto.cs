using Flownix.Backend.Contracts.DTOs.Enums;
namespace Flownix.Backend.Contracts.DTOs.ReadingDTOs
{
    public class AlertDto
    {
        public Guid Id { get; set; }
        public Guid WaterObjectId { get; set; }
        public Guid? SensorId { get; set; }
        public AlertType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public string WaterObjectName { get; set; } = string.Empty;
        public string? SensorType { get; set; }
        public string? SensorUnit { get; set; }
    }
}