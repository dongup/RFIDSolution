using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.DAL.Entities
{
    public class InventoryEntity : BaseEntity
    {
        public InventoryEntity() : base()
        {

        }

        [Key]
        public int INVENTORY_ID { get; set; }

        /// <summary>
        /// Ngày bắt đầu kiểm kê
        /// </summary>
        public DateTime INVENTORY_DATE { get; set; }

        [StringLength(100)]
        public string INVENTORY_SEQ { get; set; }

        /// <summary>
        /// Số giấy giờ yêu cầu kiểm kê
        /// </summary>
        [StringLength(100)]
        public string REF_DOC_NO { get; set; }

        /// <summary>
        /// Tên đợt kiểm kê
        /// </summary>
        [StringLength(200)]
        public string INVENTORY_NAME { get; set; }

        public InventoryStatus INVENTORY_STATUS { get; set; }

        public string COMPLETE_USER { get; set; }

        public DateTime? COMPLETE_DATE { get; set; }

        public string CANCEL_REASON { get; set; }

        public DateTime? CANCEL_DATE { get; set; }

        public string CANCEL_USER { get; set; }

        public ICollection<InventoryDetailEntity> InventoryDetails { get; set; } = new HashSet<InventoryDetailEntity>();
    }
}
