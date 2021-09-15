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
    public class TransferDetailEntity : BaseEntity
    {
        public TransferDetailEntity() : base()
        {

        }

        [Key]
        public int TRANSFER_DTL_ID { get; set; }

        public int TRANSFER_ID { get; set; }

        /// <summary>
        /// Số thứ tự ??? không hiểu tạo làm gì ???
        /// </summary>
        public int TRANSFER_DTL_EXT { get; set; }

        public int PRODUCT_ID { get; set; }

        public DateTime TRANSFER_TIME { get; set; }

        public DateTime RETURN_TIME { get; set; }

        [StringLength(100)]
        public string TRANSFER_BY { get; set; }

        [StringLength(100)]
        public string RETURN_BY { get; set; }

        [StringLength(400)]
        public string TRANSFER_NOTE { get; set; }

        [StringLength(400)]
        public string RETURN_NOTE { get; set; }

        public InoutStatus STATUS { get; set; } = InoutStatus.Borrowing;

        public GetStatus TRANSFER_STATUS { get; set; }

        public GetStatus RETURN_STATUS { get; set; }

        [ForeignKey(nameof(TRANSFER_ID))]
        public TransferEntity Transfer { get; set; }

        [ForeignKey(nameof(PRODUCT_ID))]
        public ProductEntity Product { get; set; }
    }
}
