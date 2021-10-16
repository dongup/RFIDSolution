using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFIDSolution.DataAccess.DAL.Entities;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.DAL.Entities.Identity;
using RFIDSolution.Shared.DTO;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ApiControllerBase
    {
        public RolesController(AppDbContext context, RoleManager<RoleEntity> roleManager) : base(context, roleManager: roleManager)
        {

        }

        [HttpGet]
        public async Task<ResponseModel<List<RoleModel>>> Get(string keyword)
        {
            string search = keyword?.Trim();
            var rspns = new ResponseModel<List<RoleModel>>();
            rspns.Result = await _context.Roles
                .Where(x => (string.IsNullOrEmpty(keyword)
                            || x.NormalizedName.Contains(keyword)))
                .Select(x => new RoleModel()
                {
                    Name = x.Name,
                    RoleId = x.Id
                }).ToListAsync();

            return rspns.Succeed();
        }

        [HttpPost]
        public async Task<ResponseModel<object>> Post(RoleModel value)
        {
            var rspns = new ResponseModel<object>();
            var newItem = new RoleEntity();
            //Category không được trùng tên
            if (_context.Roles.Any(x => x.Name == value.Name))
            {
                return rspns.Failed($"Role {value.Name} already existed, please try different name!");
            }
            newItem.Name = value.Name;

            await _roleManager.CreateAsync(newItem);
            return rspns.Succeed();
        }

        [HttpPut("{id}")]
        public async Task<ResponseModel<object>> Put(int id, RoleModel value)
        {
            var rspns = new ResponseModel<object>();
            var savedItem = await _roleManager.FindByIdAsync(id.ToString());
            savedItem.Name = value.Name;

            var result = await _roleManager.UpdateAsync(savedItem);
            if (result.Succeeded)
            {
                return rspns.Succeed();
            }
            else
            {
                return rspns.Failed(result.Errors.FirstOrDefault().Description);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ResponseModel<object>> Delete(int id)
        {
            var rspns = new ResponseModel<object>();

            var savedItem = _context.Roles.Find(id);
            await _roleManager.DeleteAsync(savedItem);
            return rspns.Succeed();
        }
    }
}
