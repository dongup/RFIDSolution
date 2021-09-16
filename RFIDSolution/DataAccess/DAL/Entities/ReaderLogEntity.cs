using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.DataAccess.DAL.Entities
{
    public class ReaderLogEntity
    {
        public ReaderLogEntity()
        {

        }

        [Key]
        public int Id { get; set; }

        public string LOG_CONTENT { get; set; }

        public RdrLog LOG_TYPE { get; set; }

        public DateTime CREATED_DATE { get; set; }

        public string NOTE { get; set; }

        public bool IS_DELETED { get; set; }
    }
}
