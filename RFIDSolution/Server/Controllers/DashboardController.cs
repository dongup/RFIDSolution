using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Dashboard;
using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ApiControllerBase
    {
        public DashboardController(AppDbContext context) : base(context)
        {

        }

        [HttpGet("sumary")]
        public async Task<ResponseModel<SumaryModel>> GetSumary()
        {
            var rspns = new ResponseModel<SumaryModel>();

            SumaryModel sumary = new SumaryModel();
            sumary.TotalIncompletedPlan = _context.INVENTORY
                .Where(x => x.INVENTORY_STATUS != Shared.Enums.AppEnums.InventoryStatus.Completed
                         || x.INVENTORY_STATUS != Shared.Enums.AppEnums.InventoryStatus.Canceled)
                .Count();
            sumary.TotalShoe = _context.PRODUCT.Count();
            sumary.TotalShoeInStock = _context.PRODUCT.Where(x => x.PRODUCT_STATUS == Shared.Enums.AppEnums.ProductStatus.Available || x.PRODUCT_STATUS == Shared.Enums.AppEnums.ProductStatus.Unavailable).Count();
            sumary.TotalTags = _context.RFID_TAG.Count();
            sumary.TotalTransferedShoe = _context.PRODUCT.Where(x => x.PRODUCT_STATUS == Shared.Enums.AppEnums.ProductStatus.Transfered).Count();

            return rspns.Succeed(sumary);
        }

        [HttpGet("expiredTransfer")]
        public async Task<ResponseModel<PaginationResponse<TransferInoutModel>>> GetExpiredTransfer(string keyword, int pageIndex, int pageItem)
        {
            var rspns = new ResponseModel<PaginationResponse<TransferInoutModel>>();

            int deadline = _context.CONFIG.Select(x => x.DEFAULT_TRANSFER_DEADLINE).FirstOrDefault();

            var result = _context.PRODUCT_TRANSFER
                .OrderByDescending(x => x.TIME_START)
                .Where(x => (string.IsNullOrEmpty(keyword) 
                            || x.TRANSFER_TO.Contains(keyword))
                            && x.TRANSFER_STATUS == Shared.Enums.AppEnums.InoutStatus.Borrowing)
                .Select(x => new TransferInoutModel()
                {
                    TRANSFER_TO = x.TRANSFER_TO,
                    TRANSFER_REASON = x.TRANSFER_REASON,
                    TRANSFER_STATUS = x.TRANSFER_STATUS,
                    REF_DOC_DATE = x.REF_DOC_DATE.ToVNString(),
                    REF_DOC_NO = x.REF_DOC_NO,
                    RETURN_BY = x.RETURN_BY,
                    TRANSFER_BY = x.TRANSFER_BY,
                    TRANSFER_ID = x.TRANSFER_ID,
                    TIME_START = x.TIME_START,
                    TIME_END = x.TIME_END,
                    TRANSFER_NOTE = x.TRANSFER_NOTE,
                    RETURN_NOTE = x.RETURN_NOTE,
                    NOTE = x.NOTE
                }).ToList()
                .Where(x => (DateTime.Now - x.TIME_START).TotalDays >= deadline)
                .AsQueryable();

            return rspns.Succeed(new PaginationResponse<TransferInoutModel>(result, pageItem, pageIndex));
        }
    }
}
