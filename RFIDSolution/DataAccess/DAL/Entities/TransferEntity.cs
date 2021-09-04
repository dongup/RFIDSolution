using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.DAL.Entities
{
    public class TransferEntity : BaseEntity
    {
        public TransferEntity()
        {

        }

        [Key]
        public int TRANSFER_ID { get; set; }

        public InoutStatus TRANSFER_STATUS { get; set; } = InoutStatus.Borrowing;

        [StringLength(400)]
        public string TRANSFER_REASON { get; set; }

        [StringLength(40)]
        public string TRANSFER_TO { get; set; }

        [StringLength(40)]
        public string REF_DOC_NO { get; set; }

        public DateTime? REF_DOC_DATE { get; set; }

        public string TRANSFER_BY { get; set; }

        public string RETURN_BY { get; set; }

        public string TRANSFER_NOTE { get; set; }

        public string RETURN_NOTE { get; set; }

        public DateTime TIME_START { get; set; }

        public DateTime? TIME_END { get; set; }

        public ICollection<TransferDetailEntity> TransferDetails { get; set; } = new HashSet<TransferDetailEntity>();
    }
}
