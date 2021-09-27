using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFIDSolution.DataAccess.DAL.Entities;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
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
    public class DepartmentController : ApiControllerBase
    {
        public DepartmentController(AppDbContext context) : base(context)
        {

        }

        [HttpGet]
        public async Task<ResponseModel<List<DepartmentResponse>>> Get(string keyword)
        {
            string search = keyword?.Trim();
            var rspns = new ResponseModel<List<DepartmentResponse>>();
            rspns.Result = await _context.DEPT_DEF
                .Where(x => (string.IsNullOrEmpty(keyword)
                            || x.DEPT_NAME.Contains(keyword)))
                .Select(x => new DepartmentResponse()
                {
                    DEPT_ID = x.DEPT_ID,
                    DEPT_NAME = x.DEPT_NAME,
                    USER_COUNT = x.Users.Count()
                }).ToListAsync();

            return rspns.Succeed();
        }

        [HttpPost]
        public async Task<ResponseModel<object>> Post(DepartmentRequest value)
        {
            var rspns = new ResponseModel<object>();
            var newItem = new DepartmentEntity();
            //Department không được trùng tên
            if (_context.DEPT_DEF.Any(x => x.DEPT_NAME == value.DEPT_NAME))
            {
                return rspns.Failed($"Model {value.DEPT_NAME} already existed, please try different name!");
            }

            newItem.DEPT_NAME = value.DEPT_NAME;
            _context.DEPT_DEF.Add(newItem);
            await _context.SaveChangesAsync();
            return rspns.Succeed();
        }

        [HttpPut("{id}")]
        public async Task<ResponseModel<object>> Put(int id, DepartmentRequest value)
        {
            var rspns = new ResponseModel<object>();
            //Department không được trùng tên
            if (_context.DEPT_DEF.Any(x => x.DEPT_NAME == value.DEPT_NAME && x.DEPT_ID != id))
            {
                return rspns.Failed($"Model {value.DEPT_NAME} already existed, please try different name!");
            }

            var newItem = _context.DEPT_DEF.Find(id);
            newItem.DEPT_NAME = value.DEPT_NAME;
            newItem.UPDATED_DATE = DateTime.Now;
            await _context.SaveChangesAsync();

            return rspns.Succeed();
        }

        [HttpDelete("{id}")]
        public async Task<ResponseModel<object>> Delete(int id)
        {
            var rspns = new ResponseModel<object>();

            var newItem = _context.DEPT_DEF.Find(id);
            newItem.IS_DELETED = true;
            newItem.DELETED_DATE = DateTime.Now;

            await _context.SaveChangesAsync();
            rspns.IsSuccess = true;

            return rspns.Succeed();
        }
    }
}
