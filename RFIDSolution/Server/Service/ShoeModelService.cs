using Grpc.Core;
using System.Threading.Tasks;
using System;
using System.Linq;
using RFIDSolution.Shared.Protos;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;

public class ShoeModelService : ShoeModelProto.ShoeModelProtoBase
{
	private AppDbContext _context;

    public ShoeModelService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<ShoeModelResponse> Get(ShoeModelFilter filter, ServerCallContext context)
    {
        string keyword = filter.Keyword?.Trim();
        var reply = new List<ShoeModel>();
        reply = await _context.MODEL
            .Where(x => (string.IsNullOrEmpty(keyword)
                        || x.MODEL_NAME.Contains(keyword)))
            .Select(x => new ShoeModel()
            {
                Id = x.MODEL_ID,
                Name = x.MODEL_NAME,
                ProductCount = x.Products.Count()
            }).ToListAsync();

        var rspns = new ShoeModelResponse();
		rspns.Data.AddRange(reply);

		return rspns;
	}

    public override async Task<ShoeModelResponse> Post(ShoeModelRequest item, ServerCallContext context)
    {
        var rspns = new ShoeModelResponse();
        try
        {
            var newItem = new ModelEntity();
            newItem.MODEL_NAME = item.Name;
            _context.MODEL.Add(newItem);
            await _context.SaveChangesAsync();
            rspns.IsSuccess = true;
        }catch(Exception ex)
        {
            rspns.IsSuccess = false;
            rspns.Message = ex.Message;
        }

        return rspns;
    }

    public override async Task<ShoeModelResponse> Put(ShoeModelRequest item, ServerCallContext context)
    {
        var rspns = new ShoeModelResponse();
        try
        {
            var newItem = _context.MODEL.Find(item.Id);
            newItem.MODEL_NAME = item.Name;
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

    public override async Task<ShoeModelResponse> Delete(ShoeModelRequest item, ServerCallContext context)
    {
        var rspns = new ShoeModelResponse();
        try
        {
            var newItem = _context.MODEL.Find(item.Id);
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