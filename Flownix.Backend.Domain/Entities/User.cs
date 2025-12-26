using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flownix.Backend.Domain.Common;
using Flownix.Backend.Domain.Enums;

namespace Flownix.Backend.Domain.Entities
{
    public class User : BaseEntity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public Role Role { get; set; } = Role.Operator;
        public required string PasswordHash { get; set; }

        public virtual ICollection<UserObjectAccess> ObjectAccesses { get; set; } = new List<UserObjectAccess>();
    }
}
