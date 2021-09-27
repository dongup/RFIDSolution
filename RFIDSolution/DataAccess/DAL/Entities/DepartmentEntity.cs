using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.DAL.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.DataAccess.DAL.Entities
{
    public class DepartmentEntity : BaseEntity
    {
        public DepartmentEntity()
        {

        }

        [Key]
        public int DEPT_ID { get; set; }

        public string DEPT_NAME { get; set; }

        public ICollection<UserEntity> Users { get; set; } = new HashSet<UserEntity>();
    }
}
