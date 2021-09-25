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
    public class CategoryController : ApiControllerBase
    {
        public CategoryController(AppDbContext context) : base(context)
        {

        }

        [HttpGet]
        public async Task<ResponseModel<List<CategoryResponse>>> Get(string keyword)
        {
            string search = keyword?.Trim();
            var rspns = new ResponseModel<List<CategoryResponse>>();
            rspns.Result = await _context.CAT_DEF
                .Where(x => (string.IsNullOrEmpty(keyword)
                            || x.CAT_NAME.Contains(keyword)))
                .Select(x => new CategoryResponse()
                {
                    CAT_ID = x.CAT_ID,
                    CAT_NAME = x.CAT_NAME,
                    PRODUCT_COUNT = x.Products.Count()
                }).ToListAsync();

            return rspns.Succeed();
        }

        [HttpPost]
        public async Task<ResponseModel<object>> Post(CategoryRequest value)
        {
            var rspns = new ResponseModel<object>();
            var newItem = new CategoryEntity();
            //Category không được trùng tên
            if (_context.CAT_DEF.Any(x => x.CAT_NAME == value.CAT_NAME))
            {
                return rspns.Failed($"Model {value.CAT_NAME} already existed, please try different name!");
            }

            newItem.CAT_NAME = value.CAT_NAME;
            _context.CAT_DEF.Add(newItem);
            await _context.SaveChangesAsync();
            return rspns.Succeed();
        }

        [HttpPut("{id}")]
        public async Task<ResponseModel<object>> Put(int id, CategoryRequest value)
        {
            var rspns = new ResponseModel<object>();
            //Category không được trùng tên
            if (_context.CAT_DEF.Any(x => x.CAT_NAME == value.CAT_NAME && x.CAT_ID != id))
            {
                return rspns.Failed($"Model {value.CAT_NAME} already existed, please try different name!");
            }

            var newItem = _context.CAT_DEF.Find(id);
            newItem.CAT_NAME = value.CAT_NAME;
            newItem.UPDATED_DATE = DateTime.Now;
            await _context.SaveChangesAsync();

            return rspns.Succeed();
        }

        [HttpDelete("{id}")]
        public async Task<ResponseModel<object>> Delete(int id)
        {
            var rspns = new ResponseModel<object>();

            var newItem = _context.CAT_DEF.Find(id);
            newItem.IS_DELETED = true;
            newItem.DELETED_DATE = DateTime.Now;

            await _context.SaveChangesAsync();
            rspns.IsSuccess = true;

            return rspns.Succeed();
        }
    }
}
