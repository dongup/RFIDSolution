using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Inventory
{
    public class ProductInventoryModel
    {
        public ProductInventoryModel()
        {

        }

        public int PRODUCT_ID { get; set; }

        public string EPC { get; set; }

        public string SKU { get; set; }

        public string MODEL_NAME { get; set; }

        /// <summary>
        /// Lần tìm thấy gần nhất
        /// </summary>
        public string LAST_SEEN { get; set; }

        public string COMPLETE_USER { get; set; }

        public InventoryProductStatus IVN_STATUS_ID { get; set; } = InventoryProductStatus.NotFound;

        public string IVN_STATUS { get; set; } = InventoryProductStatus.NotFound.GetDescription();
    }
}
