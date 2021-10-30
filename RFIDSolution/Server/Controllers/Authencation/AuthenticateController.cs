using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RFIDSolution.Server.Controllers;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DTO;
using RFIDSolution.Shared.Models;
using System.Threading.Tasks;

namespace BaseApiWithIdentity.Controllers.Authorization
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ApiControllerBase
    {
        private readonly RoleManager<RoleEntity> roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<UserEntity> _signInManager;

        public AuthenticateController(AppDbContext context, UserManager<UserEntity> _userManager, RoleManager<RoleEntity> roleManager, IConfiguration configuration, SignInManager<UserEntity> signInManager) 
            : base(context, _userManager)
        {
            this.roleManager = roleManager;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost]
        public ActionResult<ResponseModel<UserModel>> CheckAuthenticate()
        {
            var rspns = new ResponseModel<UserModel>();
            UserEntity user = CurrentUser;

            return rspns.Succeed(new UserModel() { 
                DepartmentId = user.DEPARTMENT_ID,
                DepartmentName = user.DepartmentName,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                UserName = user.UserName,
                Id = user.Id,
            });
        }

        [HttpGet("confirmpassword/{password}")]
        public async Task<ResponseModel<bool>> ConfirmPassword(string password)
        {
            var rspns = new ResponseModel<bool>();

            var result = await _signInManager.CheckPasswordSignInAsync(CurrentUser, password, false);
            if (result.Succeeded)
            {
                return rspns.Succeed(true);
            }
            else
            {
                return rspns.Succeed(false);
            }
        }
    }
}
