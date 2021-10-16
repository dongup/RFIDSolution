using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RFIDSolution.Server.Controllers;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Indentity;
using RFIDSolution.WebAdmin.Models;

namespace BaseApiWithIdentity.Controllers.Authorization
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly RoleManager<RoleEntity> roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly UserManager<UserEntity> _userManager;

        public LoginController(AppDbContext context,UserManager<UserEntity> _userManager, RoleManager<RoleEntity> roleManager, IConfiguration configuration) 
        {
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            this._userManager = _userManager;
        }

        [HttpPost]
        public async Task<ResponseModel<LoginResponseModel>> Login([FromBody] LoginModel model)
        {
            var rspns = new ResponseModel<LoginResponseModel>();

            var user = _context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.Department)
                .FirstOrDefault(x => x.UserName == model.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(UserClaim.FullName, user.FullName),
                    new Claim(UserClaim.Department, user.Department?.DEPT_NAME??""),
                    new Claim(UserClaim.DepartmentId, user.DEPARTMENT_ID?.ToString()??""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                DateTime expireDay = DateTime.Now.AddDays(int.Parse(_configuration["JWT:ExpiredDay"].ToString()));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: expireDay,
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var rslt = new LoginResponseModel()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ValidTo = token.ValidTo,
                    User = new UserModel()
                    {
                        Avatar = user.Avatar,
                        Email = user.Email,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Phone = user.Phone,
                        RoleName = string.Join(", ", user.UserRoles.Select(a => a.Role.Name)),
                    }
                };
                rspns.Succeed(rslt);
            }
            else
            {
                rspns.Failed("Incorrect username or password!");
            }
            return rspns;
        }
    }
}
