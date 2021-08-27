using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.DAL.Entities.Identity
{
    public class UserRoleEntity : IdentityUserRole<int>
    {
        public RoleEntity Role { get; set; }
        public UserEntity User { get; set; }

        public bool IsDeleted { get; set; }
    }
}
