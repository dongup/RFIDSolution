using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.ProductInout;
using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTransfersController : ApiControllerBase
    {
        public ProductTransfersController(AppDbContext context) : base(context)
        {

        }

        [HttpGet]
        public async Task<ResponseModel<PaginationResponse<TransferInoutModel>>> Get(string keyword = "", int pageItem = 10, int pageIndex = 0)
        {
            var rspns = new ResponseModel<PaginationResponse<TransferInoutModel>>();

            var result = _context.PRODUCT_TRANSFER
                .OrderByDescending(x => x.TIME_START)
                .Where(x => (string.IsNullOrEmpty(keyword) || x.TRANSFER_TO.Contains(keyword)))
                .Select(x => new TransferInoutModel() { 
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
                    NOTE = x.NOTE
                });

            return rspns.Succeed(new PaginationResponse<TransferInoutModel>(result, pageItem, pageIndex));
        }

        [HttpGet("{id}")]
        public async Task<ResponseModel<TransferInoutModel>> GetById(int id)
        {
            var rspns = new ResponseModel<TransferInoutModel>();

            var result = _context.PRODUCT_TRANSFER
                .OrderByDescending(x => x.TIME_START)
                .Where(x => x.TRANSFER_ID == id)
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
                    NOTE = x.NOTE,
                    Products = x.TransferDetails.Select(a => new ProductTransferModel()
                    {
                        EPC = a.Product.EPC,
                        ModelName = a.Product.Model.MODEL_NAME,
                        ProductId = a.PRODUCT_ID,
                        TRANSFER_BY = a.TRANSFER_BY,
                        TRANSFER_NOTE = a.TRANSFER_NOTE,
                        TRANSFER_TIME = a.TRANSFER_TIME,
                        STATUS = a.STATUS,
                        RETURN_BY = a.RETURN_BY,
                        RETURN_NOTE = a.RETURN_NOTE,
                        RETURN_TIME = a.RETURN_TIME,
                        SKU = a.Product.PRODUCT_CODE,
                    }).ToList()
                }).FirstOrDefault();

            return rspns.Succeed(result);
        }

        [HttpGet("byProductId/{id}")]
        public async Task<ResponseModel<TransferInoutModel>> GetByProductId(int id)
        {
            var rspns = new ResponseModel<TransferInoutModel>();

            //Tìm lần transfer mới nhất chưa được trả lại và có chứa product id gửi lên
            var result = _context.PRODUCT_TRANSFER
                .Where(x => x.TransferDetails.Any(a => a.PRODUCT_ID == id) && x.TRANSFER_STATUS == Shared.Enums.AppEnums.InoutStatus.Borrowing)
                .OrderByDescending(x => x.TIME_START)
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
                    NOTE = x.NOTE,
                    Products = x.TransferDetails.Select(a => new ProductTransferModel() {
                       EPC = a.Product.EPC,
                       ModelName = a.Product.Model.MODEL_NAME,
                       ProductId = a.PRODUCT_ID,
                       TRANSFER_BY = a.TRANSFER_BY,
                       TRANSFER_NOTE = a.TRANSFER_NOTE,
                       TRANSFER_TIME = a.TRANSFER_TIME,
                       STATUS = a.STATUS,
                       RETURN_BY = a.RETURN_BY,
                       RETURN_NOTE = a.RETURN_NOTE,
                       RETURN_TIME = a.RETURN_TIME,
                       SKU = a.Product.PRODUCT_CODE,
                    }).ToList()
                }).FirstOrDefault();

            if (result == null) return rspns.Failed();
            //Trả kết quả về
            return rspns.Succeed(result);
        }

        [HttpGet("reasons")]
        public ResponseModel<List<string>> GetReasons()
        {
            var rspns = new ResponseModel<List<string>>();

            List<string> reasons = _context.PRODUCT_TRANSFER
                //.GroupBy(x => x.TRANSFER_REASON)
                .Select(x => x.TRANSFER_REASON).ToList();
            reasons.Add("To test");
            reasons.Add("Quality control");
            reasons.Add("Develop new style");
            reasons.Add("Reason 1");
            reasons.Add("Reason 2");
            reasons.Add("Reason 3");
            reasons.Add("Reason 4");

            return rspns.Succeed(reasons);
        }

        [HttpPost("transferOut")]
        public async Task<ResponseModel<bool>> transferOut(TransferOutRequest value)
        {
            var rspns = new ResponseModel<bool>();

            //Kiểm tra mật khẩu xác nhận
            //Để sau

            //Kiểm tra cờ onhold, not avaiable
            var products = _context.PRODUCT.Where(x => value.Products.Select(a => a.ProductId).Contains(x.PRODUCT_ID));
            if(products.Any(x => x.PRODUCT_STATUS != Shared.Enums.AppEnums.ProductStatus.Available))
            {
                return rspns.Failed("One or more shoe are not avaiable for transfer out, please check again!");
            }
            
            //Lưu giữ liệu
            TransferEntity newTransfer = new TransferEntity();
            newTransfer.TIME_START = DateTime.Now;
            newTransfer.TRANSFER_REASON = value.TRANSFER_REASON;
            newTransfer.TRANSFER_TO = value.TRANSFER_TO;
            newTransfer.TRANSFER_BY = value.TRANSFER_BY;
            newTransfer.TRANSFER_STATUS = Shared.Enums.AppEnums.InoutStatus.Borrowing;
            newTransfer.REF_DOC_DATE = value.REF_DOC_DATE.ToDateTime();
            newTransfer.REF_DOC_NO = value.REF_DOC_NO;
            newTransfer.CREATED_DATE = DateTime.Now;
            newTransfer.TRANSFER_NOTE = value.REF_DOC_NO;
            _context.PRODUCT_TRANSFER.Add(newTransfer);
            _context.SaveChanges();

            List<TransferDetailEntity> transferDetails = value.Products.Select(x => new TransferDetailEntity() {
                PRODUCT_ID = x.ProductId,
                TRANSFER_ID = newTransfer.TRANSFER_ID,
                CREATED_DATE = DateTime.Now,
                TRANSFER_NOTE = x.Note,
                TRANSFER_STATUS = Shared.Enums.AppEnums.GetStatus.Ok,
                TRANSFER_TIME = DateTime.Now,
                TRANSFER_BY = value.TRANSFER_BY
            }).ToList();
            _context.PRODUCT_TRANSFER_DTL.AddRange(transferDetails);
            await _context.SaveChangesAsync();

            //Đổi trạng thái của các item được transfer sau khi đã lưu dữ liệu thành công
            foreach (var prod in products)
            {
                prod.PRODUCT_STATUS = Shared.Enums.AppEnums.ProductStatus.Transfered;
            }
            await _context.SaveChangesAsync();

            //Trả kết quả
            return rspns.Succeed();
        }

        [HttpPost("transferIn")]
        public async Task<ResponseModel<bool>> transferIn(TransferInRequest value)
        {
            var rspns = new ResponseModel<bool>();

            //Kiểm tra mật khẩu xác nhận
            //Để sau

            //Tìm thông tin đầy đủ của những sản phẩm đã scan
            var products = _context.PRODUCT.Where(x => value.Products.Select(a => a.ProductId).Contains(x.PRODUCT_ID));

            //Nếu có sản phẩm chưa được transfer out thì trả về lỗi
            if (products.Any(x => x.PRODUCT_STATUS == Shared.Enums.AppEnums.ProductStatus.Available))
            {
                return rspns.Failed("One or more shoe are not transfered out yet, please check again!");
            }

            foreach (var prod in products)
            {
                //Tìm transfer detail mới nhất của product được scan
                var transferDetail = _context.PRODUCT_TRANSFER_DTL.Where(x => x.PRODUCT_ID == prod.PRODUCT_ID).OrderByDescending(x => x.TRANSFER_TIME).FirstOrDefault();

                //Đổi trạng thái và cập nhập thông tin 
                transferDetail.STATUS = Shared.Enums.AppEnums.InoutStatus.Returned;
                transferDetail.RETURN_NOTE = prod.NOTE;
                transferDetail.RETURN_TIME = DateTime.Now;
                transferDetail.RETURN_BY = value.RETURN_BY;

                //Tìm lần transfer của transfer detail tìm được
                var transfer = _context.PRODUCT_TRANSFER
                    .Include(x => x.TransferDetails)
                    .Where(x => x.TRANSFER_ID == transferDetail.TRANSFER_ID).FirstOrDefault();

                //Đếm số lượng giày đã trả, nếu đã trả hết thì đổi trạng thái của lần inout và cập nhập thông tin
                var okShoe = transfer.TransferDetails
                    .Where(x => x.STATUS == Shared.Enums.AppEnums.InoutStatus.Returned).Count();
                if(okShoe == transfer.TransferDetails.Count)
                {
                    transfer.TRANSFER_STATUS = Shared.Enums.AppEnums.InoutStatus.Returned;
                    transfer.RETURN_NOTE += "\r\n" + value.RETURN_NOTE;
                    transfer.RETURN_BY = string.Join(", ", transfer.TransferDetails.Select(x => x.RETURN_BY).Distinct());
                    transfer.TIME_END = DateTime.Now;
                }

                //Đổi trạng thái của các item được transfer sau khi đã lưu dữ liệu thành công
                prod.PRODUCT_STATUS = Shared.Enums.AppEnums.ProductStatus.Available;
                Console.WriteLine("Returned shoe: " + prod.EPC);
            }
            await _context.SaveChangesAsync();

            //Trả kết quả
            return rspns.Succeed();
        }
    }
}
