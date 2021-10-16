using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Dashboard
{
    public class SumaryModel
    {
        public SumaryModel()
        {

        }

        public int TotalShoe { get; set; }

        public int TotalTags { get; set; }

        public int TotalTransferedShoe { get; set; }

        public int TotalShoeInStock { get; set; }

        public int TotalIncompletedPlan { get; set; }
    }
}
