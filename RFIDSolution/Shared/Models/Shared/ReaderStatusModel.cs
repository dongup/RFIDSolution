using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models.Shared
{
    public class ReaderStatusModel
    {
        public ReaderStatusModel()
        {

        }

        public bool IsConnected { get; set; }

        public bool IsInventoring { get; set; }

        public string Message { get; set; }

        public bool IsSuccess { get; set; } = false;
    }
}
