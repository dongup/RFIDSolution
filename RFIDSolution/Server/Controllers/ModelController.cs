using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;
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

            var result = _context.MODEL.Select(x => x.MODEL_NAME).ToList();
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
    }
}
