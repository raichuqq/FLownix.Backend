using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Contracts.DTOs.Enums;
namespace Flownix.Backend.Contracts.DTOs.UpdateDTOs
{
    public class UpdatePumpDto
    {
        public string? Name { get; set; }
        public Guid? WaterObjectId { get; set; } 
        public PumpStatus? Status { get; set; } 
    }
}