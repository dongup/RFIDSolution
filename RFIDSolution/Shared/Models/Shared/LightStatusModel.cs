using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models.Shared
{
    public class LightStatusModel
    {
        public LightStatusModel()
        {

        }

        public int PortIndex { get; set; }

        public GPOPortType Type { get; set; } = GPOPortType.Green;

        public bool PortState { get; set; }
    }
}
