using RFIDSolution.Shared.Models.ProductInout;
using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models
{
    public class TransferInoutModel
    {
        public TransferInoutModel()
        {

        }

        public int TRANSFER_ID { get; set; }

        public InoutStatus TRANSFER_STATUS { get; set; }

        public string statusClass => TRANSFER_STATUS == InoutStatus.Borrowing ? "badge bg-danger" : "badge bg-sucess";

        public string TRANSFER_REASON { get; set; }

        public string TRANSFER_TO { get; set; }

        public int? TRANSFER_DEPT_ID { get; set; }

        /// <summary>
        /// Người lấy
        /// </summary>
        public string TRANSFER_BY { get; set; }

        /// <summary>
        /// Người trả
        /// </summary>
        public string RETURN_BY { get; set; }

        public string REF_DOC_NO { get; set; }

        public string REF_DOC_DATE { get; set; } = DateTime.Now.ToShortVNString();

        public DateTime TIME_START { get; set; }

        public DateTime? TIME_END { get; set; }

        public string TRANSFER_NOTE { get; set; }

        public string RETURN_NOTE { get; set; }

        public string NOTE { get; set; }
        
        public string CREATED_USER { get; set; }

        public string CREATED_USER_DEPT { get; set; }

        public bool showDetail;

        public string totalTransferTime => TRANSFER_STATUS == InoutStatus.Returned? ((DateTime)TIME_END - TIME_START).ToShortTimeString() : "";

        //Danh sách sản phẩm đã transfer trong lần transfer này
        public List<ProductTransferModel> Products { get; set; } = new List<ProductTransferModel>();
    }
}
