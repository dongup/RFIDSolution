using System;
using System.Collections.Generic;
using System.Text;

namespace RFIDSolution.Shared.Models.Products
{
    public class ModelResponse
    {
        public ModelResponse()
        {

        }

        public int MODEL_ID { get; set; }

        public string MODEL_NAME { get; set; }

        public int PRODUCT_COUNT { get; set; }

        public bool IsEditting;
    }
}
