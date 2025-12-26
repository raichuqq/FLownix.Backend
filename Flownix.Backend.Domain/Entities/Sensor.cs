using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Domain.Common;
using Flownix.Backend.Domain.Enums;

namespace Flownix.Backend.Domain.Entities
{
    public class Sensor : BaseEntity
    {
        public Guid WaterObjectId { get; set; }

        public SensorType Type { get; set; }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public required string Unit { get; set; }

        public bool IsActive { get; set; } = true;

        public required virtual WaterObject WaterObject { get; set; }
        public virtual ICollection<SensorReading> Readings { get; set; } = new List<SensorReading>();
    }
}
