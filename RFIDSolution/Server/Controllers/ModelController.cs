using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class ModelController : ApiControllerBase
    {
        public ModelController(AppDbContext context) : base(context)
        {

        }

        [HttpGet("GetModels")]
        public ResponseModel<List<string>> GetModels()
        {
            var rspns = new ResponseModel<List<string>>();

            var result = _context.MODEL_DEF.Select(x => x.MODEL_NAME).ToList();
            result.Insert(0, "Any");

            return rspns.Succeed(result);
        }

        [HttpGet("GetCategories")]
        public ResponseModel<List<string>> GetCategories()
        {
            var rspns = new ResponseModel<List<string>>();

            var result = _context.PRODUCT.Select(x => x.PRODUCT_CATEGORY).ToList().Distinct().ToList();
            result.Insert(0, "Any");

            return rspns.Succeed(result);
        }

        [HttpGet]
        public async Task<ResponseModel<List<ModelResponse>>> Get(string keyword)
        {
            string search = keyword?.Trim();
            var rspns = new ResponseModel<List<ModelResponse>>();
            rspns.Result = await _context.MODEL_DEF
                .Where(x => (string.IsNullOrEmpty(keyword)
                            || x.MODEL_NAME.Contains(keyword)))
                .Select(x => new ModelResponse()
                {
                    MODEL_ID = x.MODEL_ID,
                    MODEL_NAME = x.MODEL_NAME,
                    PRODUCT_COUNT = x.Products.Count()
                }).ToListAsync();

            return rspns.Succeed();
        }

        [HttpPost]
        public async Task<ResponseModel<object>> Post(ModelRequest value)
        {
            var rspns = new ResponseModel<object>();

            //Model không được trùng tên
            if (_context.MODEL_DEF.Any(x => x.MODEL_NAME == value.MODEL_NAME))
            {
                return rspns.Failed($"Model {value.MODEL_NAME} already existed, please try different name!");
            }

            var newItem = new ModelEntity();
            newItem.MODEL_NAME = value.MODEL_NAME;
            _context.MODEL_DEF.Add(newItem);
            await _context.SaveChangesAsync();
            return rspns.Succeed();
        }

        [HttpPut("{id}")]
        public async Task<ResponseModel<object>> Put(int id, ModelRequest value)
        {
            var rspns = new ResponseModel<object>();
            var newItem = _context.MODEL_DEF.Find(id);
            if(_context.MODEL_DEF.Any(x => x.MODEL_NAME == value.MODEL_NAME && x.MODEL_ID != id))
            {
                return rspns.Failed($"Model {value.MODEL_NAME} already existed, please try different name!");
            }

            newItem.MODEL_NAME = value.MODEL_NAME;
            newItem.UPDATED_DATE = DateTime.Now;
            await _context.SaveChangesAsync();

            return rspns.Succeed();
        }

        [HttpDelete("{id}")]
        public async Task<ResponseModel<object>> Delete(int id)
        {
            var rspns = new ResponseModel<object>();

            var newItem = _context.MODEL_DEF.Find(id);
            newItem.IS_DELETED = true;
            newItem.DELETED_DATE = DateTime.Now;

            await _context.SaveChangesAsync();
            rspns.IsSuccess = true;

            return rspns.Succeed();
        }

    }
}
