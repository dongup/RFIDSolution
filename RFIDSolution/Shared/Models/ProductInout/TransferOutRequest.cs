using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.ProductInout
{
    public class TransferOutRequest
    {
        public TransferOutRequest()
        {

        }

        public TransferOutRequest(TransferInoutModel model)
        {
            TRANSFER_REASON = model.TRANSFER_REASON;
            TRANSFER_TO = model.TRANSFER_TO;
            TRANSFER_BY = model.TRANSFER_BY;
            REF_DOC_NO = model.REF_DOC_NO;
            REF_DOC_DATE = model.REF_DOC_DATE;
            TRANSFER_NOTE = model.NOTE;
            DEPARTMENT_ID = model.TRANSFER_DEPT_ID;
        }

        public string TRANSFER_REASON { get; set; }

        public string TRANSFER_TO { get; set; }

        /// <summary>
        /// Người lấy
        /// </summary>
        public string TRANSFER_BY { get; set; }

        public string REF_DOC_NO { get; set; }

        public string REF_DOC_DATE { get; set; }

        public string TRANSFER_NOTE { get; set; }

        public int? DEPARTMENT_ID { get; set; }

        public List<ProductTransferRequest> Products { get; set; } = new List<ProductTransferRequest>();
    }
}
