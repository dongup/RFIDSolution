using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Shared
{
    public class RFTagRequest
    {
        public RFTagRequest()
        {

        }

        public List<int> AntenIds { get; set; } = new List<int>();
    }
}
