using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Inventory;
using RFIDSolution.Shared.Models.Products;
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
                            || x.PRODUCT_ID.ToString() == keyword
                            || x.PRODUCT_SEASON == keyword)
                            && (shoeStatus == 0 || (int)x.PRODUCT_STATUS == shoeStatus))
                .OrderByDescending(x => x.CREATED_DATE)
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
                    CurrentLocation = x.CURRENT_LOCATION,
                    POC = x.PRODUCT_POC,
                    RefDocDate = x.REF_DOC_DATE,
                    RefDocNo = x.REF_DOC_NO,
                    Remarks = x.PRODUCT_REMARKS,
                    Season = x.PRODUCT_SEASON,
                    Size = x.PRODUCT_SIZE,
                    SKU = x.PRODUCT_CODE,
                    Stage = x.PRODUCT_STAGE,
                    Article = "",
                    ProductStatus = x.PRODUCT_STATUS,
                    CategoryId = x.CATEGORY_ID,
                    CreatedUser = x.CREATED_USER,
                    Note = x.NOTE,
                });

            return rspns.Succeed(new PaginationResponse<ProductModel>(query, pageItem, pageIndex));
        }

        [HttpGet("searchTransfer")]
        public async Task<ResponseModel<List<ProductModel>>> GetSearchForTransfer(string keyword)
        {
            var rspns = new ResponseModel<List<ProductModel>>();

            var query = _context.PRODUCT
                .Where(x => (x.PRODUCT_CODE.Contains(keyword)
                            || x.EPC == keyword
                            || x.Model.MODEL_NAME.Contains(keyword)
                            || x.Category.CAT_NAME.Contains(keyword)
                            || x.PRODUCT_SIZE.Contains(keyword)
                            || x.PRODUCT_LOCATION.Contains(keyword)
                            || x.PRODUCT_SEASON.Contains(keyword)
                            || x.PRODUCT_STAGE.Contains(keyword)
                            || x.PRODUCT_SEASON == keyword))
                .OrderByDescending(x => x.CREATED_DATE)
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
                    ProductStatus = x.PRODUCT_STATUS,
                    CategoryId = x.CATEGORY_ID,
                    CreatedUser = x.CREATED_USER,
                    Note = x.NOTE,
                });

            return rspns.Succeed(query.ToList());
        }

        [HttpGet("app")]
        public async Task<ResponseModel<List<ProductModel>>> GetOnApp(string keyword, int shoeStatus, int pageItem = 10, int pageIndex = 0)
        {
            var rspns = new ResponseModel<List<ProductModel>>();

            var query = _context.PRODUCT
                .Where(x => (string.IsNullOrEmpty(keyword)
                            || x.PRODUCT_CODE.Contains(keyword)
                            || x.EPC == keyword
                            || x.PRODUCT_SEASON == keyword)
                            && (shoeStatus == 0 || (int)x.PRODUCT_STATUS == shoeStatus))
                .OrderByDescending(x => x.CREATED_DATE)
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
                    ProductStatus = x.PRODUCT_STATUS,
                    CategoryId = x.CATEGORY_ID,
                    CreatedUser = x.CREATED_USER,
                    Note = x.NOTE,
                });
            PaginationResponse<ProductModel> pagiResponse = new PaginationResponse<ProductModel>(query, pageItem, pageIndex);
            var result = pagiResponse.Data;
            return rspns.Succeed(result);
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
                    SIZE = x.PRODUCT_SIZE,
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
                    CurrentLocation = x.CURRENT_LOCATION,
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
                    CategoryId = x.CATEGORY_ID,
                    Note = x.NOTE,
                    TransferHistory = x.TransferDetails.OrderByDescending(a => a.CREATED_DATE)
                    .Select(a => a.Transfer).Select(a => new TransferInoutModel()
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
                    }).ToList(),
                    Inventories = x.InventoryDetails.OrderByDescending(a => a.CREATED_DATE).Select(a => new InventoryModel()
                    {
                        INVENTORY_ID = a.DTL_ID,
                        INVENTORY_NAME = a.Inventory.INVENTORY_NAME,
                        CREATED_USER = a.Inventory.CREATED_USER,
                        CREATED_DATE = a.Inventory.CREATED_DATE.ToShortVNString(),
                        PRODUCT_STATUS = a.STATUS,
                        INVENTORY_STATUS_ID = a.Inventory.INVENTORY_STATUS,
                        INVENTORY_STATUS = a.Inventory.INVENTORY_STATUS.GetDescription()
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
                .OrderBy(x => x.EPC)
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
                    ProductStatus = x.PRODUCT_STATUS,
                    Note =x.NOTE,
                    CategoryId = x.CATEGORY_ID,
                }).FirstOrDefault();
            if (query == null) return rspns.NotFound($"SKU: {SKU} did not exist!");
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
                    ProductId = x.PRODUCT_ID,
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
                    Note = x.NOTE,
                    CategoryId = x.CATEGORY_ID,
                }).FirstOrDefault();

            return rspns.Succeed(query);
        }

        /// <summary>
        /// Lấy ra ngẫu nhiên 1 lượng EPC đã có trong hệ thống
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet("randomEPC/{number}")]
        public async Task<ResponseModel<List<string>>> GetRandomEPC(int number)
        {
            var rspns = new ResponseModel<List<string>>();

            var query = _context.PRODUCT
                .OrderByDescending(x => Guid.NewGuid())
                .Take(number)
                .Where(x => !string.IsNullOrEmpty(x.EPC))
                .Select(x => x.EPC)
                .ToList();

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
        public async Task<ResponseModel<bool>> Post(ProductRequestModel item)
        {
            var rspns = new ResponseModel<bool>();
         
            //Nếu tag đã tồn tại thì không cho lưu
            if(_context.PRODUCT.Any(x => x.EPC == item.EPC))
            {
                return rspns.Failed($"Shoe's EPC {item.EPC} is already exist!");
            }

            //Nếu có tồn tại 1 product có cùng SKU mà không có EPC thì update lại
            if(_context.PRODUCT.Any(x => x.PRODUCT_CODE == item.SKU && string.IsNullOrEmpty(x.EPC)))
            {
                var savedEntity = _context.PRODUCT.Where(x => x.PRODUCT_CODE == item.SKU && string.IsNullOrEmpty(x.EPC)).FirstOrDefault();
                savedEntity.EPC = item.EPC;

                if(!_context.RFID_TAG.Any(x => x.EPC == item.EPC))
                {
                    RFIDTagEntity tagEntity = new RFIDTagEntity();
                    tagEntity.EPC = item.EPC;
                    tagEntity.CREATED_DATE = DateTime.Now;
                    tagEntity.CREATED_USER = CurrentUser.FullName;
                    tagEntity.CREATED_USER_ID = CurrentUser.Id;

                    _context.RFID_TAG.Add(tagEntity);
                }

                _context.SaveChanges();
            }
            else
            {
                //Ngược lại thì thêm mới
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

                var cat = _context.CAT_DEF.FirstOrDefault(x => x.CAT_ID == item.CategoryId);
                if (cat == null) return rspns.Failed($"{item.Category} is not a valid category!");

                entity.PRODUCT_CATEGORY = cat.CAT_NAME;
                entity.CATEGORY_ID = cat.CAT_ID;
                entity.REF_DOC_NO = item.RefDocNo;
                entity.REF_DOC_DATE = item.RefDocDate;
                entity.CREATED_USER_ID = CurrentUser.Id;
                entity.CREATED_USER = CurrentUser.FullName;

                _context.PRODUCT.Add(entity);
                await _context.SaveChangesAsync();

                RFIDTagEntity tagEntity = new RFIDTagEntity();
                tagEntity.EPC = entity.EPC;
                tagEntity.CREATED_USER_ID = entity.CREATED_USER_ID;
                tagEntity.CREATED_USER = entity.CREATED_USER;
                _context.RFID_TAG.Add(tagEntity);
                _context.SaveChanges();
            }

            return rspns.Succeed();
        }

        [HttpPut("unlock/{ID}")]
        public async Task<ResponseModel<bool>> UnLockShoe(int ID)
        {
            var rspns = new ResponseModel<bool>();
            var product = _context.PRODUCT.Find(ID);
            if (product == null) return rspns.NotFound();

            product.PRODUCT_STATUS = Shared.Enums.AppEnums.ProductStatus.Available;
            _context.SaveChanges();
            return rspns.Succeed();
        }

        [HttpPut("lock/{ID}")]
        public async Task<ResponseModel<bool>> LockShoe(int ID)
        {
            var rspns = new ResponseModel<bool>();
            var product = _context.PRODUCT.Find(ID);
            if (product == null) return rspns.NotFound();

            product.PRODUCT_STATUS = Shared.Enums.AppEnums.ProductStatus.Unavailable;
            _context.SaveChanges();
            return rspns.Succeed();
        }

        /// <summary>
        /// Mapping từ app
        /// </summary>
        /// <param name="item">Thông tin giày</param>
        /// <returns></returns>
        [HttpPost("app")]
        public async Task<ResponseModel<bool>> PostApp(ProductRequestModel item)
        {
            var rspns = new ResponseModel<bool>();

            //Nếu tag đã tồn tại thì không cho lưu
            if (_context.PRODUCT.Any(x => x.EPC == item.EPC))
            {
                return rspns.Failed($"Shoe's EPC {item.EPC} is already exist!");
            }

            var entity = new ProductEntity();
            entity.PRODUCT_CODE = item.SKU;
            entity.EPC = item.EPC;

            var model = _context.MODEL_DEF.FirstOrDefault(x => x.MODEL_NAME == item.ModelName);
            if(model == null) return rspns.Failed($"{item.Category} is not a valid model!");

            entity.MODEL_ID = model.MODEL_ID;
            entity.PRODUCT_SIZE = item.Size;
            entity.PRODUCT_POC = item.POC;
            entity.PRODUCT_LOCATION = item.Location;
            entity.PRODUCT_REMARKS = item.Remarks;
            entity.DEV_NAME = item.DevStyleName;
            entity.PRODUCT_SEASON = item.Season;
            entity.PRODUCT_STAGE = item.Stage;
            entity.COLOR_NAME = item.ColorWay;

            var cat = _context.CAT_DEF.FirstOrDefault(x => x.CAT_NAME == item.Category);
            if (cat == null) return rspns.Failed($"{item.Category} is not a valid category!");

            entity.PRODUCT_CATEGORY = cat.CAT_NAME;
            entity.CATEGORY_ID = cat.CAT_ID;
            entity.REF_DOC_NO = item.RefDocNo;
            entity.REF_DOC_DATE = item.RefDocDate;
            entity.CREATED_USER_ID = CurrentUser.Id;
            entity.CREATED_USER = CurrentUser.FullName;

            _context.PRODUCT.Add(entity);
            await _context.SaveChangesAsync();

            RFIDTagEntity tagEntity = new RFIDTagEntity();
            tagEntity.EPC = entity.EPC;
            tagEntity.CREATED_USER_ID = entity.CREATED_USER_ID;
            tagEntity.CREATED_USER = entity.CREATED_USER;
            _context.RFID_TAG.Add(tagEntity);
            _context.SaveChanges();

            return rspns.Succeed();
        }

        [HttpPut("{id}")]
        public async Task<ResponseModel<bool>> Put(int id, ProductModel value)
        {
            var rspns = new ResponseModel<bool>();

            var savedItem = _context.PRODUCT.Find(id);
            savedItem.PRODUCT_CODE = value.SKU;
            savedItem.EPC = value.EPC;
            savedItem.MODEL_ID = value.ModelId;
            savedItem.PRODUCT_SIZE = value.Size;
            savedItem.PRODUCT_POC = value.POC;
            savedItem.PRODUCT_LOCATION = value.Location;
            savedItem.PRODUCT_REMARKS = value.Remarks;
            savedItem.DEV_NAME = value.DevStyleName;
            savedItem.PRODUCT_SEASON = value.Season;
            savedItem.PRODUCT_STAGE = value.Stage;
            savedItem.COLOR_NAME = value.ColorWay;
            savedItem.PRODUCT_CATEGORY = _context.CAT_DEF.Find(value.CategoryId)?.CAT_NAME;
            savedItem.CATEGORY_ID = value.CategoryId;
            savedItem.REF_DOC_NO = value.RefDocNo;
            savedItem.REF_DOC_DATE = value.RefDocDate;
            savedItem.CREATED_USER_ID = CurrentUser.Id;
            savedItem.CREATED_USER = CurrentUser.FullName;

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
