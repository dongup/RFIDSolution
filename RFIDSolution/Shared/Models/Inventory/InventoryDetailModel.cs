using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Inventory
{
    public class InventoryDetailModel
    {
        public InventoryDetailModel()
        {

        }

        public int DTL_ID { get; set; }

        public int INVENTORY_ID { get; set; }

        public int PRODUCT_ID { get; set; }

        public int? COMPLETE_USER { get; set; }

        public InventoryProductStatus STATUS { get; set; }
    }
}
