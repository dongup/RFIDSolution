using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.ProductInout
{
    public class TransferInRequest
    {
        public TransferInRequest()
        {

        }

        public TransferInRequest(TransferInoutModel model, List<ProductModel> products)
        {
            RETURN_NOTE = model.RETURN_NOTE;
            RETURN_BY = model.RETURN_BY;
            this.Products = products.Select(x => new ProductTransferModel()
            {
                ProductId = x.ID,
                RETURN_NOTE = x.Note,
            }).ToList();
        }

        /// <summary>
        /// Người trả
        /// </summary>
        public string RETURN_BY { get; set; }

        /// <summary>
        /// Ghi chú khi trả
        /// </summary>
        public string RETURN_NOTE { get; set; }

        public List<ProductTransferModel> Products { get; set; } = new List<ProductTransferModel>();
    }
}
