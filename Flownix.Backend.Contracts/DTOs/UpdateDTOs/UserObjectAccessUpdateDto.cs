using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flownix.Backend.Contracts.DTOs.UpdateDTOs
{
    public class UserObjectAccessUpdateDto
    {
        public Guid? UserId { get; set; } 
        public Guid? WaterObjectId { get; set; } 
    }
}