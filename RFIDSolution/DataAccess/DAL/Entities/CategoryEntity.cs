using RFIDSolution.Shared.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.DataAccess.DAL.Entities
{
    public class CategoryEntity : BaseEntity
    {
        public CategoryEntity()
        {

        }

        [Key]
        public int CAT_ID { get; set; }

        public string CAT_NAME { get; set; }

        public ICollection<ProductEntity> Products { get; set; } = new HashSet<ProductEntity>();
    }
}
