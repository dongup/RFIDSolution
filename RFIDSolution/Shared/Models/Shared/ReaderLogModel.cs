using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Shared
{
    public class ReaderLogModel
    {
        public ReaderLogModel()
        {

        }

        public int Id { get; set; }

        public string LOG_CONTENT { get; set; }

        public RdrLog LOG_TYPE { get; set; }

        public DateTime CREATED_DATE { get; set; }

        public string NOTE { get; set; }
    }
}
