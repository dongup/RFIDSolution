using RFIDSolution.WebAdmin.DAL;
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
    public class BaseUserModel
    {
        public BaseUserModel()
        {

        }

        public BaseUserModel(UserEntity us)
        {
            if (us == null)
            {
                return;
            }

            this.Id = us.Id;
            this.FullName = us.FullName;
            this.UserName = us.UserName;
            this.Avatar = us.Avatar;
            this.CreatedDate = us.CreatedDate;
            this.CreatedUserId = us.CreatedUserId;
            this.UpdatedDate = us.UpdatedDate;
            this.UpdatedUserId = us.UpdatedUserId;
            this.Phone = us.PhoneNumber;
            this.Email = us.Email;
            RoleName = us.UserRoles.FirstOrDefault()?.Role?.Name;
        }

        public UserEntity CopyTo(UserEntity user)
        {
            user.UserName = this.UserName;
            user.FullName = this.FullName;
            user.Avatar = this.Avatar;
            user.Email = this.Email;
            user.PhoneNumber = this.Phone;

            return user;
        }

        protected int Id { get; set; }

        [Required(ErrorMessage = "User name is required!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        protected string Password { get; set; } = "";

        [Required(ErrorMessage = "Full name is required!")]
        public string FullName { get; set; }
        public string Avatar { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        public string RoleName { get; set; }

        protected DateTime CreatedDate { get; set; }
        protected int? CreatedUserId { get; set; }

        protected DateTime UpdatedDate { get; set; }
        protected int UpdatedUserId { get; set; }
    }

    public class AddUserDTO : BaseUserModel
    {
        public AddUserDTO()
        {

        }

        public AddUserDTO(UserEntity us) : base(us)
        {

        }

        [Required(ErrorMessage = "Password is required!")]
        public new string Password { get => base.Password; set => base.Password = value; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Username is requried!")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is requried!")]
        public string Password { get; set; }
    }
}
