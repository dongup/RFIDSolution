using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Products
{
    public class CategoryRequest
    {
        public CategoryRequest()
        {
                
        }

        public int CAT_ID { get; set; }

        public string CAT_NAME { get; set; }
    }
}
