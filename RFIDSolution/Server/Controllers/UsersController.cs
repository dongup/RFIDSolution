using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.Models;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        IWebHostEnvironment _webEnv;

        public UsersController(AppDbContext context, UserManager<UserEntity> userManager, RoleManager<RoleEntity> roleManager, IWebHostEnvironment hostingEnvironment) 
            : base(context, userManager, roleManager: roleManager)
        {
            _webEnv = hostingEnvironment;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ResponseModel<PaginationResponse<UserModel>>> GetUsers(string keyword, int pageItem, int pageIndex)
        {
            var rspns = new ResponseModel<PaginationResponse<UserModel>>();

            var res = _context.Users
                .Where(x => string.IsNullOrEmpty(keyword)
                            || x.NormalizedUserName.Contains(keyword)
                            || x.FullName.Contains(keyword)
                            || x.NormalizedEmail.Contains(keyword)
                            || x.PhoneNumber.Contains(keyword))
                .Select(x => new UserModel() {
                    Avatar = x.Avatar,
                    Email = x.Email,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    RoleName = string.Join(", ", x.UserRoles.Select(a => a.Role.Name))
                })
                .AsQueryable();

            return rspns.Succeed(new PaginationResponse<UserModel>(res, pageItem, pageIndex));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<UserModel>>> GetById(int id)
        {
            var rspns = new ResponseModel<UserModel>();

            var user = await _context.Users
                .Where(x => x.Id == id)
                .Select(x => new UserModel() {
                    Avatar = x.Avatar,
                    Email = x.Email,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    RoleName = string.Join(", ", x.UserRoles.Select(a => a.Role.Name)),
                })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return rspns.Failed("User does not exist!");
            }

            return rspns.Succeed(user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ResponseModel<object>> PutUserEntity(int id, UserRequestModel value)
        {
            var rspns = new ResponseModel<object>();

            if (!UserEntityExists(id))
            {
                return rspns.Failed("User does not exist!");
            }

            UserEntity user = await _context.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();

            user.FullName = value.FullName;
            user.Email = value.Email;
            user.Note = value.Note;
            user.Phone = value.Phone;


            //user.Avatar = SaveImage(user.Avatar);

            //try
            //{
            //    RoleEntity newRole = await _roleManager.FindByNameAsync(userDTO.RoleName);
            //    if(newRole == null)
            //    {
            //        throw new Exception("Please select a valid role!");
            //    }

            //    string oldRole = user.UserRoles.FirstOrDefault()?.Role?.Name;
            //    if (oldRole != "" && oldRole != null)
            //    {
            //        await _userManager.RemoveFromRoleAsync(user, oldRole);
            //    }

            //    await _userManager.AddToRoleAsync(user, newRole.Name);

            //    await _context.SaveChangesAsync();
            //    rspns.Succeed();
            //}
            //catch (Exception ex)
            //{
            //    rspns.Failed(ex.Message);
            //}

            return rspns.Succeed();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<ResponseModel<object>>> PostUserEntity(UserRequestModel value)
        {
            var rspns = new ResponseModel<object>();
            var user = new UserEntity();
            user.FullName = value.FullName;
            user.Email = value.Email;
            user.Note = value.Note;
            user.Phone = value.Phone;
            user.UserName = value.UserName;

            var result =  await _userManager.CreateAsync(user, value.Password);
            if (result.Succeeded)
            {
                return rspns.Succeed();
            }
            else
            {
                return rspns.Failed(result.Errors.FirstOrDefault().Description);
            }
        }

        private string SaveImage(string file)
        {
            if (!file.Contains("base64")) return file;

            string base64 = file.Split(',')[1];
            string fileInfo = file.Split(',')[0];
            string fileExtension = fileInfo.Split(';')[0].Split('/')[1];

            //string user = CurrentUser.UserName;
            string user = "admin";
            string fileName = $"{Guid.NewGuid().ToString()}.{fileExtension}";

            string savePath = Path.Combine(_webEnv.ContentRootPath, "wwwroot", "Upload", "Avatar", user);
            string filePath = Path.Combine(savePath, fileName);

            string relativePath = $"/Upload/Avatar/{user}/{fileName}";

            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            byte[] bytes = Convert.FromBase64String(base64);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    ms.WriteTo(fs);
                }
            }

            return relativePath;
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ResponseModel<object>> DeleteUserEntity(int id)
        {
            var rspns = new ResponseModel<object>();
            var userEntity = await _context.Users.FindAsync(id);
            if (userEntity == null)
            {
                return rspns.Failed("User does not exist!");
            }

            await _userManager.DeleteAsync(userEntity);
            rspns.Succeed();
            return rspns;
        }

        private bool UserEntityExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
