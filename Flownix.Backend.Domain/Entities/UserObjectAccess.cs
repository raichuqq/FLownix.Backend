using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Domain.Common;

namespace Flownix.Backend.Domain.Entities
{
    public class UserObjectAccess : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid WaterObjectId { get; set; }

        public required virtual User User { get; set; }
        public required virtual WaterObject WaterObject { get; set; }
    }
}
