using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.DTO
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
