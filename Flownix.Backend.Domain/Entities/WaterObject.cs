using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Domain.Common;
using Flownix.Backend.Domain.Enums;

namespace Flownix.Backend.Domain.Entities
{
    public class WaterObject : BaseEntity
    {
        public required string Name { get; set; }
        public WaterObjectType Type { get; set; }

        public required string Location { get; set; }

        public double MaxVolume { get; set; }
        public double CurrentVolume { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
        public virtual ICollection<Pump> Pumps { get; set; } = new List<Pump>();
        public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();
        public virtual ICollection<UserObjectAccess> UserAccesses { get; set; } = new List<UserObjectAccess>();
    }
}
