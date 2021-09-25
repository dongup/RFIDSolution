using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Inventory
{
    public class InventoryModel
    {
        private string iVN_RESULT;

        public InventoryModel()
        {

        }

        public int INVENTORY_ID { get; set; }

        /// <summary>
        /// Ngày kiểm kê
        /// </summary>
        public string INVENTORY_DATE { get; set; }

        /// <summary>
        /// Ngày kiểm kê
        /// </summary>
        public DateTime D_INVENTORY_DATE { get; set; }

        public string TIME_AGO { get; set; }

        public string INVENTORY_SEQ { get; set; }

        /// <summary>
        /// Số giấy giờ yêu cầu kiểm kê
        /// </summary>
        public string REF_DOC_NO { get; set; }

        /// <summary>
        /// Tên đợt kiểm kê
        /// </summary>
        public string INVENTORY_NAME { get; set; }

        public string INVENTORY_RESULT
        {
            get {
                return $"{TOTAL_FOUND}/{TOTAL}";
            }
            set => iVN_RESULT = value;
        }

        public int TOTAL_FOUND { get; set; }

        public int TOTAL { get; set; }

        public string CREATED_DATE { get; set; }

        public DateTime D_CREATED_DATE { get; set; }

        public InventoryStatus INVENTORY_STATUS_ID { get; set; }

        public string INVENTORY_STATUS { get; set; }

        public string CREATED_USER { get; set; }

        public string NOTE { get; set; }

        public string COMPLETE_USER { get; set; }

        public List<ProductInventoryModel> InventoryProducts { get; set; } = new List<ProductInventoryModel>();
    }
}
