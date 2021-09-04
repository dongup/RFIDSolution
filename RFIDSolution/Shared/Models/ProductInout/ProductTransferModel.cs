using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.ProductInout
{
    /// <summary>
    /// Chứa thông tin cơ bản của 1 product đang nằm trong 1 lần transfer
    /// </summary>
    public class ProductTransferModel
    {
        public ProductTransferModel()
        {

        }


        public int ProductId { get; set; }

        public string EPC { get; set; }

        public string SKU { get; set; }

        public string ModelName { get; set; }

        public DateTime TRANSFER_TIME { get; set; }

        public DateTime RETURN_TIME { get; set; }

        public string TRANSFER_BY { get; set; }

        public string RETURN_BY { get; set; }

        public string TRANSFER_NOTE { get; set; }

        public string RETURN_NOTE { get; set; }
    }
}
