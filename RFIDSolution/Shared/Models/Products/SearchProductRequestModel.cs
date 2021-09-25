using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models
{
    public class SearchProductRequestModel
    {
        public SearchProductRequestModel()
        {

        }

        public string SKU { get; set; }
        public string ModelName { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
    }
}
