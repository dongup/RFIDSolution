using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.DAL.Shared
{
    public class ConfigurationEntity
    {
        public ConfigurationEntity()
        {

        }

        [Key]
        public int CONFIG_ID { get; set; }

        public string READER_IP { get; set; }

        public int READER_PORT { get; set; }

        public int READER_PERIOD { get; set; }

        public int READER_TIMEOUT { get; set; }

        public int TAG_TIME_OUT { get; set; }

        /// <summary>
        /// Số ngày mượn tối đa
        /// </summary>
        public int DEFAULT_TRANSFER_DEADLINE { get; set; } = 30;

        /// <summary>
        /// Số giây đèn led sẽ sáng khi có cảnh báo, nếu = 0 nghĩa là sáng tới lúc bị tắt thì thôi
        /// </summary>
        public int GPO_RESET_TIME { get; set; }

        public bool IS_DELETED { get; set; }
    }
}
