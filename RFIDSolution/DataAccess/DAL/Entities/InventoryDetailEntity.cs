using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.DAL.Entities
{
    public class InventoryDetailEntity : BaseEntity
    {
        public InventoryDetailEntity() : base()
        {

        }

        [Key]
        public int DTL_ID { get; set; }

        public int INVENTORY_ID { get; set; }

        public int PRODUCT_ID { get; set; }

        [StringLength(50)]
        public int COMPLETE_USER { get; set; }

        public InventoryProductStatus STATUS { get; set; }
    }
}
