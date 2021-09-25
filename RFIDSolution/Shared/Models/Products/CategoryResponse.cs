using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Products
{
    public class CategoryResponse
    {
        public CategoryResponse()
        {
                
        }

        public int CAT_ID { get; set; }

        public string CAT_NAME { get; set; }

        public int PRODUCT_COUNT { get; set; }

        public bool IsEditting;
    }
}
