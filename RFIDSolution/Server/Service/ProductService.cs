using Grpc.Core;
using System.Threading.Tasks;
using System;
using System.Linq;
using RFIDSolution.Shared.Protos;
using System.Collections.Generic;
using RFIDSolution.WebAdmin.DAL;
using RFIDSolution.WebAdmin.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using RFIDSolution.WebAdmin.Utils;

public class ProductService : ProductProto.ProductProtoBase
{
    private AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<ProductResponse> Get(ProductFilter filter, ServerCallContext context)
    {
        string keyword = filter.Keyword?.Trim();
        var reply = new List<ProductModel>();
        reply = await _context.PRODUCT
            .Where(x => (string.IsNullOrEmpty(keyword)
                        || x.PRODUCT_CODE.Contains(keyword)
                        || x.EPC == keyword))
            .Select(x => new ProductModel()
            {
                ID = x.PRODUCT_ID,
                Category = x.PRODUCT_CATEGORY,
                ColorWay = x.COLOR_NAME,
                DevStyleName = x.DEV_NAME,
                EPC = x.EPC,
                Location = x.PRODUCT_LOCATION,
                LR = (int)x.LR,
                LRStr = x.LR.ToStringVN(),
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
                StatusStr = x.PRODUCT_STATUS.ToStringVN(),
                StatusId = (int)x.PRODUCT_STATUS
            }).ToListAsync();

        var rspns = new ProductResponse();
        rspns.Data.AddRange(reply);

        return rspns;
    }

    public override async Task<ProductResponse> Post(ProductRequest item, ServerCallContext context)
    {
        var rspns = new ProductResponse();
        try
        {
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

            foreach (var tag in item.EPCS)
            {
                var newItem = new ProductEntity();
                ModelUtils.CopyProperty(entity, newItem);
                newItem.EPC = tag;

                _context.PRODUCT.Add(newItem);
            }
            await _context.SaveChangesAsync();
            rspns.IsSuccess = true;
        }
        catch (Exception ex)
        {
            rspns.IsSuccess = false;
            rspns.Message = ex.Message;
        }

        return rspns;
    }

    public override async Task<ProductResponse> Put(ProductRequest item, ServerCallContext context)
    {
        var rspns = new ProductResponse();
        try
        {
            var newItem = _context.PRODUCT.Find(item.ID);
            newItem.MODEL_ID = item.ModelId;
            await _context.SaveChangesAsync();
            rspns.IsSuccess = true;
        }
        catch (Exception ex)
        {
            rspns.IsSuccess = false;
            rspns.Message = ex.Message;
        }

        return rspns;
    }

    public override async Task<ProductResponse> Delete(ProductRequest item, ServerCallContext context)
    {
        var rspns = new ProductResponse();
        try
        {
            var newItem = _context.PRODUCT.Find(item.ID);
            newItem.IS_DELETED = true;
            newItem.DELETED_DATE = DateTime.Now;

            await _context.SaveChangesAsync();
            rspns.IsSuccess = true;
        }
        catch (Exception ex)
        {
            rspns.IsSuccess = false;
            rspns.Message = ex.Message;
        }

        return rspns;
    }
}