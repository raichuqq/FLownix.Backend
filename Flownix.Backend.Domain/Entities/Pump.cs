using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Flownix.Backend.Domain.Common;
using Flownix.Backend.Domain.Enums;

namespace Flownix.Backend.Domain.Entities
{
    public class Pump : BaseEntity
    {
        public Guid WaterObjectId { get; set; }

        public required string Name { get; set; }

        public PumpStatus Status { get; set; } = PumpStatus.Off;

        public DateTime? LastUpdated { get; set; }

        public required virtual WaterObject WaterObject { get; set; }
    }
}
