using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.WebAdmin.Enums.AppEnums;

namespace RFIDSolution.WebAdmin.DAL.Entities
{
    public class RFIDTagEntity : BaseEntity
    {
        public RFIDTagEntity() : base()
        {

        }

        [Key]
        public int RFID_TAG_ID { get; set; }

        [StringLength(150)]
        public string EPC { get; set; }

        public RFIDTagStatus TAG_STATUS { get; set; }
    }
}
