using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ApiControllerBase
    {
        public ProductsController(AppDbContext context) : base(context)
        {

        }

        [HttpGet]
        public async Task<ResponseModel<PaginationResponse<ProductModel>>> Get(string keyword, int statusId, int pageItem = 10, int pageIndex = 0)
        {
            var rspns = new ResponseModel<PaginationResponse<ProductModel>>();

            var query = _context.PRODUCT
                .Where(x => (string.IsNullOrEmpty(keyword)
                            || x.PRODUCT_CODE.Contains(keyword)
                            || x.EPC == keyword
                            || x.PRODUCT_SEASON == keyword)
                            && (statusId == 0 || (int)x.PRODUCT_STATUS == statusId))
                .Select(x => new ProductModel() {
                    ID = x.PRODUCT_ID,
                    Category = x.PRODUCT_CATEGORY,
                    ColorWay = x.COLOR_NAME,
                    DevStyleName = x.DEV_NAME,
                    EPC = x.EPC,
                    Location = x.PRODUCT_LOCATION,
                    LR = x.LR,
                    LRStr = x.LR.GetDescription(),
                    ModelId = x.MODEL_ID,
                    ModelName = x.Model.MODEL_NAME,
                    POC = x.PRODUCT_POC,
                    RefDocDate = x.REF_DOC_DATE,
                    RefDocNo = x.REF_DOC_NO,
                    Remarks = x.PRODUCT_REMARKS,
                    Season = x.PRODUCT_SEASON,
                    Size = x.PRODUCT_SIZE,
                    SKU = x.PRODUCT_CODE,
                    Stage = x.PRODUCT_STAGE,
                    Article = "",
                    ProductStatus = x.PRODUCT_STATUS
                });

            return rspns.Succeed(new PaginationResponse<ProductModel>(query, pageItem, pageIndex));
        }

        [HttpGet("bySKU")]
        public async Task<ResponseModel<ProductModel>> GetBySKU(string SKU)
        {
            var rspns = new ResponseModel<ProductModel>();

            var query = _context.PRODUCT
                .Where(x => x.PRODUCT_CODE == SKU)
                .Select(x => new ProductModel()
                {
                    ID = x.PRODUCT_ID,
                    Category = x.PRODUCT_CATEGORY,
                    ColorWay = x.COLOR_NAME,
                    DevStyleName = x.DEV_NAME,
                    EPC = x.EPC,
                    Location = x.PRODUCT_LOCATION,
                    LR = x.LR,
                    LRStr = x.LR.GetDescription(),
                    ModelId = x.MODEL_ID,
                    ModelName = x.Model.MODEL_NAME,
                    POC = x.PRODUCT_POC,
                    RefDocDate = x.REF_DOC_DATE,
                    RefDocNo = x.REF_DOC_NO,
                    Remarks = x.PRODUCT_REMARKS,
                    Season = x.PRODUCT_SEASON,
                    Size = x.PRODUCT_SIZE,
                    SKU = x.PRODUCT_CODE,
                    Stage = x.PRODUCT_STAGE,
                    Article = "",
                    ProductStatus = x.PRODUCT_STATUS
                }).FirstOrDefault();
            if (query == null) return rspns.NotFound("");
            return rspns.Succeed(query);
        }

        [HttpGet("byEPC")]
        public async Task<ResponseModel<ProductModel>> GetByEPC(string EPC)
        {
            var rspns = new ResponseModel<ProductModel>();

            var query = _context.PRODUCT
                .Where(x => x.EPC == EPC)
                .Select(x => new ProductModel()
                {
                    ID = x.PRODUCT_ID,
                    Category = x.PRODUCT_CATEGORY,
                    ColorWay = x.COLOR_NAME,
                    DevStyleName = x.DEV_NAME,
                    EPC = x.EPC,
                    Location = x.PRODUCT_LOCATION,
                    LR = x.LR,
                    LRStr = x.LR.GetDescription(),
                    ModelId = x.MODEL_ID,
                    ModelName = x.Model.MODEL_NAME,
                    POC = x.PRODUCT_POC,
                    RefDocDate = x.REF_DOC_DATE,
                    RefDocNo = x.REF_DOC_NO,
                    Remarks = x.PRODUCT_REMARKS,
                    Season = x.PRODUCT_SEASON,
                    Size = x.PRODUCT_SIZE,
                    SKU = x.PRODUCT_CODE,
                    Stage = x.PRODUCT_STAGE,
                    Article = "",
                    ProductStatus = x.PRODUCT_STATUS
                }).FirstOrDefault();

            return rspns.Succeed(query);
        }

        [HttpPost]
        public async Task<ResponseModel<bool>> Post(ProductModel item)
        {
            var rspns = new ResponseModel<bool>();
         
            //Nếu tag đã tồn tại thì không cho lưu
            if(_context.PRODUCT.Any(x => x.EPC == item.EPC))
            {
                return rspns.Failed($"Shoe's EPC {item.EPC} is already exist!");
            }

            var entity = new ProductEntity();
            entity.PRODUCT_CODE = item.SKU;
            entity.EPC = item.EPC;
            entity.MODEL_ID = item.ModelId;
            entity.PRODUCT_SIZE = item.Size;
            entity.PRODUCT_POC = item.POC;
            entity.PRODUCT_LOCATION = item.Location;
            entity.PRODUCT_REMARKS = item.Remarks;
            entity.DEV_NAME = item.DevStyleName;
            entity.PRODUCT_SEASON = item.Season;
            entity.PRODUCT_STAGE = item.Stage;
            entity.COLOR_NAME = item.ColorWay;
            entity.PRODUCT_CATEGORY = item.Category;
            entity.REF_DOC_NO = item.RefDocNo;
            entity.REF_DOC_DATE = item.RefDocDate;

            _context.PRODUCT.Add(entity);
            await _context.SaveChangesAsync();

            return rspns.Succeed();
        }

        [HttpPut("{id}")]
        public async Task<ResponseModel<bool>> Put(int id, ProductModel item)
        {
            var rspns = new ResponseModel<bool>();

            var newItem = _context.PRODUCT.Find(id);
            newItem.MODEL_ID = item.ModelId;


            await _context.SaveChangesAsync();
         

            return rspns.Succeed();
        }

        [HttpDelete("{id}")]
        public async Task<ResponseModel<bool>> Delete(int id)
        {
            var rspns = new ResponseModel<bool>();

            var newItem = _context.PRODUCT.Find(id);
            newItem.IS_DELETED = true;
            newItem.DELETED_DATE = DateTime.Now;
            await _context.SaveChangesAsync();

            return rspns.Succeed();
        }
    }
}
