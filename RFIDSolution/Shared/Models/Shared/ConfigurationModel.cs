using System.ComponentModel.DataAnnotations;

namespace RFIDSolution.Shared.Models.Shared
{
    public class ConfigurationModel
    {
        public ConfigurationModel()
        {

        }

        [Key]
        public int CONFIG_ID { get; set; }

        public string READER_IP { get; set; }

        public int READER_PORT { get; set; }

        public int READER_PERIOD { get; set; }

        public int READER_TIMEOUT { get; set; }

        public bool IS_DELETED { get; set; }
    }
}
