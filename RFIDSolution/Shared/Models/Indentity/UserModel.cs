using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models
{
    public class UserModel
    {
        public UserModel()
        {

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

        public string Note { get; set; }

        protected DateTime CreatedDate { get; set; }
        protected int? CreatedUserId { get; set; }

        protected DateTime UpdatedDate { get; set; }
        protected int UpdatedUserId { get; set; }
    }

    public class UserRequestModel : UserModel
    {
        public UserRequestModel()
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
