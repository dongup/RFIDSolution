using RFIDSolution.WebAdmin.DAL.Entities;
using RFIDSolution.WebAdmin.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.DTO
{
    public class RoleModel : BaseDTO
    {
        [Required(ErrorMessage = "Please provide role's name!")]
        public string Name { get; set; }
        protected string ConcurentStamp { get; set; }

        public RoleModel()
        {

        }

        public RoleModel(RoleEntity role)
        {
            Id = role.Id;
            Name = role.Name;
            ConcurentStamp = role.ConcurrencyStamp;
        }

        public RoleEntity ToEntity(RoleEntity role)
        {
            role.Name = Name;
            return role;
        }
    }

}
