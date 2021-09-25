using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RFIDSolution.Server.Controllers;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DTO;
using RFIDSolution.Shared.Models;

namespace BaseApiWithIdentity.Controllers.Authorization
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ApiControllerBase
    {
        private readonly RoleManager<RoleEntity> roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(AppDbContext context, UserManager<UserEntity> _userManager, RoleManager<RoleEntity> roleManager, IConfiguration configuration) : base(context, _userManager)
        {
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        public ActionResult<ResponseModel<UserEntity>> CheckAuthenticate()
        {
            var rspns = new ResponseModel<UserEntity>();

            return rspns.Succeed(CurrentUser);
        }
    }
}
