using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.WebAdmin.Enums.AppEnums;

namespace RFIDSolution.WebAdmin.DAL.Entities
{
    public class ProductInoutDetailEntity : BaseEntity
    {
        public ProductInoutDetailEntity() : base()
        {

        }

        [Key]
        public int IO_DTL_ID { get; set; }

        public int IO_ID { get; set; }

        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int IO_DTL_EXT { get; set; }

        public int PRODUCT_ID { get; set; }

        public DateTime IO_GET_TIME { get; set; }

        public DateTime IO_RET_TIME { get; set; }

        [StringLength(100)]
        public string IO_GET_USER { get; set; }

        [StringLength(100)]
        public string IO_RET_USER { get; set; }

        [StringLength(400)]
        public string IO_GET_NOTE { get; set; }

        [StringLength(400)]
        public string IO_RET_NOTE { get; set; }

        public GetStatus IO_GET_STATUS { get; set; }

        public GetStatus IO_RET_STATUS { get; set; }
    }
}
