using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RFIDSolution.Server.Controllers;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.Models;

namespace BaseApiWithIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangePasswordController : ApiControllerBase
    {

        public ChangePasswordController(AppDbContext context, UserManager<UserEntity> userManager)
        {
        }


        // PUT: api/UserPassword/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ResponseModel<object>> ChangePassword(ChangePasswordModel info)
        {
            var rspns = new ResponseModel<object>();
            UserEntity user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                rspns.Failed("User does not exist!");
                return rspns;
            }

            try
            {
                IdentityResult rslt = await _userManager.ChangePasswordAsync(user, info.Password, info.NewPassword);
                if (rslt.Succeeded)
                {
                    rspns.Succeed();
                }
                else
                {
                    rspns.Failed(rslt.Errors.First().Description);
                }
            }
            catch (Exception ex)
            {
                rspns.Failed(ex.Message);
            }

            return rspns.Succeed();
        }

    }
}
