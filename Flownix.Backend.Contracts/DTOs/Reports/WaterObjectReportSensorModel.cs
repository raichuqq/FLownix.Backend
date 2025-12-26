using System;

namespace Flownix.Backend.Contracts.DTOs.Reports
{
    public class WaterObjectReportSensorModel
    {
        public required string SensorTypeName { get; set; }
        public DateTime RecordedAt { get; set; }
        public string? ValueName { get; set; }
        public double? ValueNumeric { get; set; }
        public string? Unit { get; set; }
        public required string ReadingStatus { get; set; }
        public string? Note { get; set; }
    }
}