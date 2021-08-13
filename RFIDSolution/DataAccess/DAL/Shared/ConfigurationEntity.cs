using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.DAL.Shared
{
    public class ConfigurationEntity
    {
        public ConfigurationEntity()
        {

        }

        [Key]
        public int CONFIG_ID { get; set; }

        public string KEY { get; set; }

        public string VALUE { get; set; }

        public bool IS_DELETED { get; set; }
    }
}
