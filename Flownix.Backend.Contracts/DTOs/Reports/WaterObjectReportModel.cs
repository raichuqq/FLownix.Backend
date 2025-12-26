using System;
using System.Collections.Generic;

namespace Flownix.Backend.Contracts.DTOs.Reports
{
    public class WaterObjectReportModel
    {
        public Guid WaterObjectId { get; set; }
        public required string WaterObjectName { get; set; }
        public DateTime ReportDate { get; set; }

        public string? Location { get; set; }
        public string WaterObjectType { get; set; } = string.Empty; 

        public required string UserEmail { get; set; }
        public required string UserName { get; set; }

        public required string CurrentStatus { get; set; }
        public double CurrentVolume { get; set; }
        public double MaxVolume { get; set; }
        public double VolumePercentage => MaxVolume > 0 ? (CurrentVolume / MaxVolume) * 100 : 0;

        public double TemperatureMin { get; set; }
        public double TemperatureMax { get; set; }
        public double WaterLevelMin { get; set; }
        public double WaterLevelMax { get; set; }

        public List<WaterObjectReportSensorModel> SensorReadings { get; set; } = new();

        public required string SummaryConclusion { get; set; }
    }
}