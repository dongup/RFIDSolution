using RFIDSolution.WebAdmin.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.DTO
{
    public class BaseDTO
    {
        public BaseDTO()
        {

        }

        protected int Id { get; set; } = 0;

        protected DateTime CreatedDate { get; set; }
        protected int CreatedUserId { get; set; }

        protected DateTime UpdatedDate { get; set; }
        protected int UpdatedUserId { get; set; }
    }
}
