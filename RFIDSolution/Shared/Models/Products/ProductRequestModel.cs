using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Products
{
    public class ProductRequestModel
    {
        public ProductRequestModel()
        {

        }

        public int ID { get; set; }
        public string SKU { get; set; }
        public string EPC { get; set; }
        public int? ModelId { get; set; }
        public string Article { get; set; }
        public string Size { get; set; }
        public string POC { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
        public string DevStyleName { get; set; }
        public string Season { get; set; }
        public string Stage { get; set; }
        public string ColorWay { get; set; }
        public string Category { get; set; }
        public int? CategoryId { get; set; }
        public string ModelName { get; set; }
        public string RefDocNo { get; set; }
        public string RefDocDate { get; set; }
        public int LR { get; set; }
        public string LRStr { get; set; }
        public string ProductStatusStr { get; set; }
        public string Note { get; set; }

    }
}
