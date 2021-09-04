using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Shared
{
    public class AntenaModel
    {
        public AntenaModel()
        {

        }

        /// <summary>
        /// ID của hệ thống
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Id của anten trên đầu đọc
        /// </summary>
        public string ANTENNA_ID { get; set; }

        /// <summary>
        /// Tên anten do người dùng đặt
        /// </summary>
        public string ANTENNA_NAME { get; set; }

        /// <summary>
        /// Vị trí đặt anten do người dùng config
        /// </summary>
        public AntennaLoction LOCATION { get; set; } = AntennaLoction.Table;

        /// <summary>
        /// Cường độ của anten
        /// </summary>
        public int ANTENNA_POWER { get; set; }
    }
}
