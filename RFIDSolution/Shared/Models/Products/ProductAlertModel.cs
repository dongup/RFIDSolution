using System;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Products
{
    public class ProductAlertModel
    {
        public ProductAlertModel()
        {

        }

        public int ALERT_ID { get; set; }

        public int PRODUCT_ID { get; set; }

        public string MODEL_NAME { get; set; }

        public string CATEGORY { get; set; }

        public string EPC { get; set; }

        public string SKU { get; set; }

        public string SIZE { get; set; }

        public string COLOR { get; set; }

        public string ALERT_IP { get; set; }

        public DateTime ALERT_TIME { get; set; }

        public int ALERT_FREQ { get; set; }

        public AlterConfirmStatus ALERT_CONF_STATUS { get; set; }

        public string ALERT_CONF_REASON { get; set; }

        public string ALERT_CONF_USER { get; set; }

        public DateTime? ALERT_CONF_TIME { get; set; }

        public string WARNING_TIME { get; set; }

        public int TotalWarningSecond { get; set; }
    }
}
