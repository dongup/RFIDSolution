using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RFIDSolution.Server.Controllers;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;

namespace BaseApiWithIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ApiControllerBase
    {
        private SignInManager<UserEntity> _signInManager;

        public LogoutController(AppDbContext context, SignInManager<UserEntity> signInManager) : base (context)
        {
            this._signInManager = signInManager;
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ResponseModel<object>>> Logout()
        {
            var rspns = new ResponseModel<object>();

            await _signInManager.SignOutAsync();
            return rspns.Succeed();
        }
    }
}
