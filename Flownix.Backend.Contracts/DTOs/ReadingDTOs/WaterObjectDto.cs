using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Contracts.DTOs.Enums;
using Flownix.Backend.Contracts.DTOs.ReadingDTOs;

namespace Flownix.Backend.Contracts.DTOs.ReadingDTOs
{
    public class WaterObjectDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public WaterObjectType Type { get; set; }
        public required string Location { get; set; }
        public double MaxVolume { get; set; }
        public double CurrentVolume { get; set; }
        public bool IsActive { get; set; }
    }
}

