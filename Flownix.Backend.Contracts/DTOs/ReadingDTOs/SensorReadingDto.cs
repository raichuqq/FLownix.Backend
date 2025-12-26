namespace Flownix.Backend.Contracts.DTOs.ReadingDTOs
{
    public class SensorReadingDto
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public double Value { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}