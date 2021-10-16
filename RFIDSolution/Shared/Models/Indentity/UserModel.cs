using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.Models
{
    public class UserModel
    {
        public UserModel()
        {

        }

        public int Id { get; set; }

        public string UserName { get; set; }

        protected string Password { get; set; } = "";

        public string FullName { get; set; }
        public string Avatar { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string RoleName { get; set; }
        public string DepartmentName { get; set; }
        public int? DepartmentId { get; set; }

        public string Note { get; set; }

        public UserStatus Status {get;set;}

        public int activeTab = 1;
        public bool ShowDetail;

        public List<LogModel> Logs { get; set; } = new List<LogModel>();
        public List<RoleModel> Roles { get; set; } = new List<RoleModel>();

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
