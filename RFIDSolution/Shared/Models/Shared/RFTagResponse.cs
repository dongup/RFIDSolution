using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Shared
{
    public class RFTagResponse
    {
        public RFTagResponse()
        {

        }

        public string EPCID { get; set; }
        public int RSSI { get; set; }
        public int SignalStrenght { get; set; }
        public short RelativeDistance { get; set; }
        public int AntennaID { get; set; }
        public int Count { get; set; }
        public long FirstSeen { get; set; }
        public long LastSeen { get; set; }
        public int ScanCount { get; set; }
        public int Status { get; set; }
    }
}
