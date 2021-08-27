using RFIDSolution.Shared.DAL.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models
{
    public class LogResponse : LogEntity
    {
        public LogResponse() : base()
        {

        }

        private string FormatBody()
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(RequestBody);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }
    }
}
