using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flownix.Backend.Contracts.DTOs.CreateDTOs
{
    public class UserObjectAccessCreateDto
    {
        public required Guid UserId { get; set; }
        public required Guid WaterObjectId { get; set; }
    }
}