using System;
using System.ComponentModel.DataAnnotations;
using static RFIDSolution.WebAdmin.Enums.AppEnums;

namespace RFIDSolution.WebAdmin.DAL.Entities
{
    public class InventoryEntity : BaseEntity
    {
        public InventoryEntity() : base()
        {

        }

        [Key]
        public int IVENTORY_ID { get; set; }

        public DateTime IVENTORY_DATE { get; set; }

        [StringLength(100)]
        public string IVENTORY_SEQ { get; set; }

        [StringLength(100)]
        public string REF_DOC_NO { get; set; }

        [StringLength(200)]
        public string INVENTORY_NAME { get; set; }

        public InventoryStatus INVENTORY_STATUS { get; set; }

        public string COMPLETE_USER { get; set; }
    }
}
