using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Domain.Common;

namespace Flownix.Backend.Domain.Entities
{
    public class SensorReading : BaseEntity
    {
        public Guid SensorId { get; set; }

        public double Value { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

        public required virtual Sensor Sensor { get; set; }
    }
}
