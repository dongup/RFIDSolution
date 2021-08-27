using RFIDSolution.Shared.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace RFIDSolution.Shared.DAL.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            CREATED_DATE = DateTime.Now;
        }

        public DateTime CREATED_DATE { get; set; }
        public int CREATED_USER_ID { get; set; }

        public DateTime UPDATED_DATE { get; set; }
        public int UPDATETED_USER_ID { get; set; }

        public DateTime DELETED_DATE { get; set; }
        public bool IS_DELETED { get; set; }
    }
}
