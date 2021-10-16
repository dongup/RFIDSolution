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
    public class ProductAlertEntity : BaseEntity
    {
        public ProductAlertEntity() : base()
        {

        }

        [Key]
        public int ALERT_ID { get; set; }

        public int PRODUCT_ID { get; set; }

        [StringLength(150)]
        [Required]
        public string EPC { get; set; }

        [StringLength(150)]
        public string ALERT_IP { get; set; }

        public DateTime ALERT_TIME { get; set; }

        public int ALERT_FREQ { get; set; }

        public AlterConfirmStatus ALERT_CONF_STATUS { get; set; } = AlterConfirmStatus.Pending;

        public string ALERT_CONF_REASON { get; set; }

        public string ALERT_CONF_USER { get; set; }

        public DateTime? ALERT_CONF_TIME { get; set; }

        [ForeignKey(nameof(PRODUCT_ID))]
        public ProductEntity Product { get; set; }
    }
}
