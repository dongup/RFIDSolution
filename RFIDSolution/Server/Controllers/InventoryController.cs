using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Inventory;
using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ApiControllerBase
    {
        public InventoryController(AppDbContext context) : base(context)
        {

        }

        /// <summary>
        /// Lấy ra những kế hoặc kiểm kê chưa đóng để show cho app mobile
        /// </summary>
        /// <returns></returns>
        [HttpGet("ongoing")]
        public ResponseModel<List<InventoryModel>> GetOnGoing()
        {
            var rspns = new ResponseModel<List<InventoryModel>>();

            var result = _context.INVENTORY
                .Where(x => x.INVENTORY_STATUS == InventoryStatus.Pending
                || x.INVENTORY_STATUS == InventoryStatus.OnGoing)
                .OrderByDescending(x => x.CREATED_DATE)
                .Select(x => new InventoryModel()
                {
                    INVENTORY_ID = x.INVENTORY_ID,
                    COMPLETE_USER = x.COMPLETE_USER,
                    INVENTORY_STATUS = x.INVENTORY_STATUS.GetDescription(),
                    INVENTORY_STATUS_ID = x.INVENTORY_STATUS,
                    CREATED_DATE = x.INVENTORY_DATE.ToVNString(),
                    D_CREATED_DATE =x.CREATED_DATE,
                    D_INVENTORY_DATE = x.INVENTORY_DATE,
                    INVENTORY_NAME = x.INVENTORY_NAME,
                    INVENTORY_SEQ = x.INVENTORY_SEQ,
                    REF_DOC_NO = x.REF_DOC_NO,
                    INVENTORY_DATE =x.CREATED_DATE.ToVNString(),
                    TOTAL_FOUND = x.InventoryDetails.Where(a => a.STATUS == InventoryProductStatus.Found).Count(),
                    TOTAL = x.InventoryDetails.Count(),
                    TIME_AGO = x.CREATED_DATE.TimeAgo()
                }).ToList();

            return rspns.Succeed(result);
        }

        /// <summary>
        /// Lấy ra chi tiết kế hoach kiểm kê theo id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ResponseModel<InventoryModel> GetById(int id)
        {
            var rspns = new ResponseModel<InventoryModel>();

            var result = _context.INVENTORY
                .Where(x => x.INVENTORY_ID == id)
                .OrderByDescending(x => x.CREATED_DATE)
                .Select(x => new InventoryModel()
                {
                    INVENTORY_ID = x.INVENTORY_ID,
                    COMPLETE_USER = x.COMPLETE_USER,
                    INVENTORY_STATUS = x.INVENTORY_STATUS.GetDescription(),
                    INVENTORY_STATUS_ID = x.INVENTORY_STATUS,
                    CREATED_DATE = x.INVENTORY_DATE.ToVNString(),
                    D_CREATED_DATE = x.CREATED_DATE,
                    D_INVENTORY_DATE = x.INVENTORY_DATE,
                    INVENTORY_NAME = x.INVENTORY_NAME,
                    INVENTORY_SEQ = x.INVENTORY_SEQ,
                    REF_DOC_NO = x.REF_DOC_NO,
                    INVENTORY_DATE = x.CREATED_DATE.ToVNString(),
                    TOTAL_FOUND = x.InventoryDetails.Where(a => a.STATUS == InventoryProductStatus.Found).Count(),
                    TOTAL = x.InventoryDetails.Count(),
                    TIME_AGO = x.CREATED_DATE.TimeAgo(),
                    CREATED_USER = x.CREATED_USER,
                    NOTE = x.NOTE,
                    InventoryProducts = x.InventoryDetails
                    .Select(a => new ProductInventoryModel()
                    {
                        DTL_ID = a.DTL_ID,
                        INV_STATUS_ID = a.STATUS,
                        INV_STATUS = a.STATUS.GetDescription(),
                        COMPLETE_USER = "",
                        EPC = a.Product.EPC,
                        MODEL_NAME = a.Product.Model.MODEL_NAME,
                        PRODUCT_ID = a.Product.PRODUCT_ID,
                        SKU = a.Product.PRODUCT_CODE,
                        CATEGORY = a.Product.PRODUCT_CATEGORY,
                        COLOR = a.Product.COLOR_NAME,
                        LOCATION = a.Product.PRODUCT_LOCATION,
                        SIZE = a.Product.PRODUCT_SIZE
                    }).ToList()
                }).FirstOrDefault();
            if (result == null)
                return rspns.NotFound();

            return rspns.Succeed(result);
        }

        [HttpPut("{id}")]
        public ResponseModel<object> updateInventoryResult(int id, InventoryModel value)
        {
            ResponseModel<object> rspns = new ResponseModel<object>();

            var productInvt = _context.INVENTORY_DTL.Where(x => x.INVENTORY_ID == id);
            foreach(var item in productInvt)
            {
                var newItem = value.InventoryProducts.FirstOrDefault(x => x.DTL_ID == item.DTL_ID);
                if(newItem != null)
                {
                    if(item.STATUS == InventoryProductStatus.NotFound)
                    {
                        item.STATUS = newItem.INV_STATUS_ID;
                        item.FOUND_DATE = DateTime.Now;
                    }
                    item.UPDATED_DATE = DateTime.Now;
                }
            }
            _context.SaveChanges();
            return rspns.Succeed();
        }
    }
}
