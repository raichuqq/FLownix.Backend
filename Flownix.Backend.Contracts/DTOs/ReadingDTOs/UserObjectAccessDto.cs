using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Flownix.Backend.Contracts.DTOs.ReadingDTOs;

namespace Flownix.Backend.Contracts.DTOs.ReadingDTOs
{
    public class UserObjectAccessDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid WaterObjectId { get; set; }
    }
}