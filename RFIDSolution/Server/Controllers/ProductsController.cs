using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Inventory;
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
        public async Task<ResponseModel<PaginationResponse<ProductModel>>> Get(string keyword, int shoeStatus, int pageItem = 10, int pageIndex = 0)
        {
            var rspns = new ResponseModel<PaginationResponse<ProductModel>>();

            var query = _context.PRODUCT
                .Where(x => (string.IsNullOrEmpty(keyword)
                            || x.PRODUCT_CODE.Contains(keyword)
                            || x.EPC == keyword
                            || x.PRODUCT_SEASON == keyword)
                            && (shoeStatus == 0 || (int)x.PRODUCT_STATUS == shoeStatus))
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

        [HttpGet("all")]
        public async Task<ResponseModel<List<ProductInventoryModel>>> GetAll()
        {
            var rspns = new ResponseModel<List<ProductInventoryModel>>();

            var query = _context.PRODUCT
                .Select(x => new ProductInventoryModel()
                {
                    INV_STATUS_ID = Shared.Enums.AppEnums.InventoryProductStatus.NotFound,
                    INV_STATUS = Shared.Enums.AppEnums.InventoryProductStatus.NotFound.GetDescription(),
                    COMPLETE_USER = "",
                    EPC = x.EPC,
                    MODEL_NAME = x.Model.MODEL_NAME,
                    PRODUCT_ID = x.PRODUCT_ID,
                    SKU = x.PRODUCT_CODE,
                    CATEGORY = x.PRODUCT_CATEGORY,
                    COLOR = x.COLOR_NAME,
                    LOCATION = x.PRODUCT_LOCATION,
                    SIZE = x.PRODUCT_SIZE
                }).ToList();

            //List<ProductInventoryModel> models = new List<ProductInventoryModel>();
            //string[] epcs = new string[] { "E28011606000020D895CCA97",
            //                "E28011606000020D895CCC57",
            //                "E28011606000020D895CCA77",
            //                "E28011606000020D895CCC47",
            //                "E28011606000020D895CCAB7",
            //                "E28011606000020D895CCC07",
            //                "1236" };

            //for (int i = 0; i < 4000; i++)
            //{
            //    models.Add(new ProductInventoryModel()
            //    {
            //        SKU = epcs.OrderBy(x => Guid.NewGuid()).FirstOrDefault(),
            //        EPC = epcs.OrderBy(x => Guid.NewGuid()).FirstOrDefault()
            //    });
            //}

            return rspns.Succeed(query);
        }

        [HttpGet("{id}")]
        public async Task<ResponseModel<ProductModel>> GetById(int id)
        {
            var rspns = new ResponseModel<ProductModel>();

            var query = _context.PRODUCT
                .Where(x => x.PRODUCT_ID == id)
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
                    CreatedUser = x.CREATED_USER,
                    Article = "",
                    ProductStatus = x.PRODUCT_STATUS,
                    TransferHistory = x.TransferDetails.Select(a => a.Transfer).Select(a => new TransferInoutModel()
                    {
                        NOTE = a.NOTE,
                        REF_DOC_DATE = a.REF_DOC_DATE.ToShortVNString(),
                        REF_DOC_NO = a.REF_DOC_NO,
                        RETURN_BY = a.RETURN_BY,
                        RETURN_NOTE = a.RETURN_NOTE,
                        TRANSFER_REASON = a.TRANSFER_REASON,
                        TIME_END = a.TIME_END,
                        TIME_START = a.TIME_START,
                        TRANSFER_BY = a.TRANSFER_BY,
                        TRANSFER_ID = a.TRANSFER_ID,
                        TRANSFER_NOTE = a.TRANSFER_NOTE,
                        TRANSFER_STATUS = a.TRANSFER_STATUS,
                        TRANSFER_TO = a.TRANSFER_TO,
                    }).ToList()
                }).FirstOrDefault();
            if (query == null) return rspns.NotFound("");
            return rspns.Succeed(query);
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
                    CreatedUser = x.CREATED_USER,
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
                    CreatedUser = x.CREATED_USER,
                    Article = "",
                    ProductStatus = x.PRODUCT_STATUS
                }).FirstOrDefault();

            return rspns.Succeed(query);
        }

        [HttpPost("search")]
        public async Task<ResponseModel<List<ProductInventoryModel>>> GetForSearch(SearchProductRequestModel value)
        {
            var rspns = new ResponseModel<List<ProductInventoryModel>>();

            var query = _context.PRODUCT
                .Where(x => (string.IsNullOrEmpty(value.SKU) || x.PRODUCT_CODE.Contains(value.SKU))
                            && (string.IsNullOrEmpty(value.ModelName) || value.ModelName == "Any" || x.Model.MODEL_NAME == value.ModelName) 
                            && (string.IsNullOrEmpty(value.CategoryName) || value.CategoryName == "Any" || x.PRODUCT_CATEGORY == value.CategoryName)
                            && (string.IsNullOrEmpty(value.Color) || x.COLOR_NAME.Contains(value.Color))
                            && (string.IsNullOrEmpty(value.Size) || x.PRODUCT_SIZE.Contains(value.Size)))
                .Select(x => new ProductInventoryModel()
                {
                    INV_STATUS_ID = Shared.Enums.AppEnums.InventoryProductStatus.NotFound,
                    INV_STATUS = Shared.Enums.AppEnums.InventoryProductStatus.NotFound.GetDescription(),
                    COMPLETE_USER = "",
                    EPC = x.EPC,
                    MODEL_NAME = x.Model.MODEL_NAME,
                    PRODUCT_ID = x.PRODUCT_ID,
                    SKU = x.PRODUCT_CODE,
                    CATEGORY = x.PRODUCT_CATEGORY,
                    COLOR = x.COLOR_NAME,
                    LOCATION = x.PRODUCT_LOCATION,
                    SIZE = x.PRODUCT_SIZE
                }).ToList();

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
            entity.CREATED_USER_ID = CurrentUser.Id;
            entity.CREATED_USER = CurrentUser.FullName;

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
