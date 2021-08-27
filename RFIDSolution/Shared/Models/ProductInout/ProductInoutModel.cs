using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models
{
    public class ProductInoutModel
    {
        public ProductInoutModel()
        {

        }

        public int IO_ID { get; set; }

        public InoutStatus IO_STATUS { get; set; }

        public string IO_REASON { get; set; }

        public string IO_DEPART { get; set; }

        /// <summary>
        /// Người lấy
        /// </summary>
        public string TAKE_USER { get; set; }

        /// <summary>
        /// Người trả
        /// </summary>
        public string RETURN_USER { get; set; }

        public string REF_DOC_NO { get; set; }

        public string REF_DOC_DATE { get; set; }
    }
}
