using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Products
{
    public class DepartmentResponse
    {
        public DepartmentResponse()
        {
                
        }

        public int DEPT_ID { get; set; }

        public string DEPT_NAME { get; set; }

        public int USER_COUNT { get; set; }

        public bool IsEditting;
    }
}
