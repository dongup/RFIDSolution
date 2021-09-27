using Microsoft.AspNetCore.Identity;
using RFIDSolution.DataAccess.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.DAL.Entities.Identity
{
    public class UserEntity : IdentityUser<int>
    {
        public string Avatar { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public bool IsAdmin { get; set; }

        public bool IS_DELETED { get; set; }

        public string Note { get; set; }

        public int? DEPARTMENT_ID { get; set; }

        [ForeignKey(nameof(DEPARTMENT_ID))]
        public DepartmentEntity Department { get; set; }

        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime CreatedDate { get; set; }

        public int? CreatedUserId { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int UpdatedUserId { get; set; }

        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
    }
}
