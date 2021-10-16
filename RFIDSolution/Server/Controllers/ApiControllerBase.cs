using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities.Identity;
using System;
using System.Data;
using System.Linq;

namespace RFIDSolution.Server.Controllers
{
    [Authorize]
    public class ApiControllerBase : ControllerBase
    {
        private UserEntity currentUser { get; set; }

        public DateTime now => DateTime.Now;
        public int CurrentUserId => CurrentUser.Id;

        public UserEntity CurrentUser
        {
            get
            {
                var user = HttpContext.Items["User"];
                if (user == null)
                {
                    //user = new UserEntity() { Id = 0 };
                    throw new Exception("Bạn chưa đăng nhập hoặc phiên làm việc của bạn đã hết hạn, xin vui lòng đăng nhập lại!!!");
                }
                return (UserEntity)user;
            }
        }

        public readonly AppDbContext _context;
        public readonly UserManager<UserEntity> _userManager;
        public readonly RoleManager<RoleEntity> _roleManager;
        public readonly IWebHostEnvironment _env;

        //public int ScaleId = currentUser.ScaleId;
        public int ScaleId = 1;

        public ApiControllerBase(AppDbContext context = null,
            UserManager<UserEntity> userManager = null,
            RoleManager<RoleEntity> roleManager = null,
            IWebHostEnvironment webEnv = null)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _env = webEnv;
        }

        public ApiControllerBase()
        {

        }

    }
}
