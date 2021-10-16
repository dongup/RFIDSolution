using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using RFIDSolution.Server.Controllers;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;

namespace BaseApiWithIdentity.Controllers.Authorization
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResetPasswordController : ApiControllerBase
    {
        private SignInManager<UserEntity> _signInManager;

        public ResetPasswordController(AppDbContext context, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : base(context, userManager)
        {
            this._signInManager = signInManager;
        }

        // PUT: api/ResetPassword/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("{userId}")]
        public async Task<ResponseModel<bool>> ResetPassword(int userId, ResetPwdModel info)
        {
            var rspns = new ResponseModel<bool>();
            UserEntity user = _context.Users.Find(userId);

            if (user == null) return rspns.Failed("User does not exist!");
           
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            IdentityResult rslt = await _userManager.ResetPasswordAsync(user, token, info.NewPassword);
            if (rslt.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return rspns.Succeed(true);
            }
            else
            {
                return rspns.Failed(rslt.Errors.First().Description);
            }
        }
    }

    public class ResetPwdModel
    {
        public string NewPassword { get; set; }
    }
}
