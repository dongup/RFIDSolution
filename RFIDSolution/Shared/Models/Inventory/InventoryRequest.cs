using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Inventory
{
    public class InventoryRequest
    {
        public InventoryRequest()
        {

        }

        public string INVENTORY_NAME { get; set; }

        public string REF_DOC_NO { get; set; }

        public string REMARKS { get; set; }

        public List<int> EXCLUDED_PRODUCTS { get; set; } = new List<int>();
    }
}
