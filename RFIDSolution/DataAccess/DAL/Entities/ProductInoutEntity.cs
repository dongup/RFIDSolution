using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.DAL.Entities
{
    public class ProductInoutEntity
    {
        public ProductInoutEntity()
        {

        }

        [Key]
        public int IO_ID { get; set; }

        public InoutStatus IO_STATUS { get; set; }

        [StringLength(400)]
        public string IO_REASON { get; set; }

        [StringLength(40)]
        public string IO_DEPART { get; set; }

        [StringLength(40)]
        public string REF_DOC_NO { get; set; }

        public DateTime REF_DOC_DATE { get; set; }
    }
}
