using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Domain.Common;
using Flownix.Backend.Domain.Enums;

namespace Flownix.Backend.Domain.Entities
{
    public class Alert : BaseEntity
    {
        public Guid WaterObjectId { get; set; }
        public Guid? SensorId { get; set; }

        public AlertType Type { get; set; }
        public required string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public required virtual WaterObject WaterObject { get; set; }
        public virtual Sensor? Sensor { get; set; }
    }
}
