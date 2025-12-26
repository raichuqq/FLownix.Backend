namespace Flownix.Backend.Contracts.DTOs.CreateDTOs
{
    public class CreateSensorReadingDto
    {
        public required Guid SensorId { get; set; }
        public required double Value { get; set; }
    }
}