using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Flownix.Backend.Contracts.DTOs.Enums;

namespace Flownix.Backend.Contracts.DTOs.ReadingDTOs
{
    public class PumpDto
    {
        public Guid Id { get; set; }
        public Guid WaterObjectId { get; set; }

        public string Name { get; set; } = null!;

        public PumpStatus Status { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
