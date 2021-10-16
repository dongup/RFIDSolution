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
using RFIDSolution.Shared.DTO;
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
                    Id = x.Id,
                    Status = x.Status,
                    Avatar = x.Avatar,
                    Email = x.Email,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Note = x.Note,
                    DepartmentName = x.Department.DEPT_NAME,
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
                    Id = x.Id,
                    Status = x.Status,
                    Avatar = x.Avatar,
                    Email = x.Email,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Note = x.Note,
                    DepartmentName = x.Department.DEPT_NAME,
                    //RoleName = string.Join(", ", x.UserRoles.Select(a => a.Role.Name)),
                    //Roles = x.UserRoles.Select(a => new RoleModel()
                    //{
                    //    Name = a.Role.Name,
                    //    RoleId = a.Role.Id, 
                    //}).ToList(),
                    //Logs = x.Logs.Select(x => new Shared.DAL.Entities.LogModel()
                    //{
                    //    LogContent = x.LogContent,
                    //    ExceptionMessage = x.ExceptionMessage,
                    //    RequestUrl = x.RequestUrl,
                    //    RequestIpAddress = x.RequestIpAddress,
                    //    RequestBody = x.RequestBody,
                    //    CreatedDate = x.CREATED_DATE
                    //}).ToList()
                })
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return rspns.Failed("User does not exist!");
            }

            return rspns.Succeed(user);
        }

        // GET: api/Users/5
        [HttpGet("byUserName/{userName}")]
        public async Task<ActionResult<ResponseModel<UserModel>>> GetByUserName(string userName)
        {
            var rspns = new ResponseModel<UserModel>();

            var user = await _context.Users
                .Where(x => x.UserName == userName)
                .Select(x => new UserModel()
                {
                    Id = x.Id,
                    Status = x.Status,
                    Avatar = x.Avatar,
                    Email = x.Email,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    Note = x.Note,
                    DepartmentName = x.Department.DEPT_NAME,
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
        public async Task<ResponseModel<object>> PutUserEntity(int id, UserModel value)
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
            user.DEPARTMENT_ID = value.DepartmentId;
            user.DepartmentName = _context.DEPT_DEF.Find(value.DepartmentId).DEPT_NAME;
            user.Note = value.Note;

            _context.SaveChanges();
            return rspns.Succeed();
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<ResponseModel<object>>> PostUserEntity(UserRequestModel value)
        {
            var rspns = new ResponseModel<object>();
            var user = new UserEntity();
            user.FullName = value.FullName;
            user.Status = Shared.Enums.AppEnums.UserStatus.Active;
            user.DEPARTMENT_ID = value.DepartmentId;
            user.DepartmentName = _context.DEPT_DEF.Find(value.DepartmentId).DEPT_NAME;
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

        // POST: api/Users/Role/5/4
        [HttpPost("role/{userid}/{roleId}")]
        public async Task<ActionResult<ResponseModel<object>>> AddRoleToUser(int userId, int roleId)
        {
            var rspns = new ResponseModel<object>();
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var role = _context.Roles.Where(x => x.Id == roleId).FirstOrDefault();

            var result = await _userManager.AddToRoleAsync(user, role.Name);
            if (result.Succeeded)
            {
                return rspns.Succeed();
            }
            else
            {
                return rspns.Failed(result.Errors.FirstOrDefault().Description);
            }
        }

        // POST: api/Users/Role/5/4
        [HttpDelete("role/{userId}/{roleId}")]
        public async Task<ResponseModel<object>> RemoveUserRole(int userId, int roleId)
        {
            var rspns = new ResponseModel<object>();
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            var role = _context.Roles.Where(x => x.Id == roleId).FirstOrDefault();

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
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
        public async Task<ResponseModel<bool>> DeleteUserEntity(int id)
        {
            var rspns = new ResponseModel<bool>();
            var userEntity = _context.Users.Find(id);
            if (userEntity == null)
            {
                return rspns.Failed("User does not exist!");
            }

            await _userManager.DeleteAsync(userEntity);
            return rspns.Succeed();
        }

        private bool UserEntityExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
