using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public int? COMPLETE_USER { get; set; }

        public InventoryProductStatus STATUS { get; set; }

        public DateTime? FOUND_DATE { get; set; }

        [ForeignKey(nameof(PRODUCT_ID))]
        public ProductEntity Product { get; set; }

        [ForeignKey(nameof(INVENTORY_ID))]
        public InventoryEntity Inventory { get; set; }
    }
}
