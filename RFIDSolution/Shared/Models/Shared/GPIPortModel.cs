using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Shared
{
    public class GPIPortModel
    {
        public GPIPortModel()
        {

        }

        public int Index { get; set; }

        public GPIPortStatus Status { get; set; }
    }
}
