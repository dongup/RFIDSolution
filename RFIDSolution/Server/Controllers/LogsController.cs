using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Entities;
using RFIDSolution.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ApiControllerBase
    {
        public LogsController(AppDbContext context) :base(context)
        {

        }

        [HttpGet]
        public async Task<ResponseModel<PaginationResponse<LogModel>>> Get(string keyword, int level, int pageItem, int pageIndex)
        {
            var rspns = new ResponseModel<PaginationResponse<LogModel>>();

            var logs = _context.LOG
                .Where(x => (string.IsNullOrEmpty(keyword) 
                            || x.LogContent.Contains(keyword)
                            || x.RequestIpAddress.Contains(keyword)
                            || x.RequestUrl.Contains(keyword))
                            && (level == 0 || (int)x.Level == level))
                .OrderByDescending(x => x.CREATED_DATE)
                .Select(x => new LogModel()
                {
                    CreatedDate = x.CREATED_DATE,
                    ExceptionDetail = x.ExceptionDetail,
                    ExceptionMessage = x.ExceptionMessage,
                    Level = x.Level,
                    Id = x.Id,
                    LogContent = x.LogContent,
                    Method = x.Method,
                    RequestIpAddress = x.RequestIpAddress,
                    RequestUrl = x.RequestUrl,
                    RequestUser = x.RequestUser,
                    RequestUserId = x.RequestUserId,
                    OperationSystemVersion = x.OperationSystemVersion,
                    Token = x.Token,
                    UserAgent = x.UserAgent
                });

            return rspns.Succeed(new PaginationResponse<LogModel>(logs, pageItem, pageIndex));
        }

        [HttpGet("{id}")]
        public async Task<ResponseModel<LogModel>> Get(int id)
        {
            var rspns = new ResponseModel<LogModel>();

            var logs = _context.LOG
                .Where(x => x.Id == id)
                .OrderByDescending(x => x.CREATED_DATE)
                .Select(x => new LogModel()
                {
                    CreatedDate = x.CREATED_DATE,
                    ExceptionDetail = x.ExceptionDetail,
                    ExceptionMessage = x.ExceptionMessage,
                    Level = x.Level,
                    Id = x.Id,
                    LogContent = x.LogContent,
                    Method = x.Method,
                    RequestIpAddress = x.RequestIpAddress,
                    RequestUrl = x.RequestUrl,
                    RequestUser = x.RequestUser,
                    RequestUserId = x.RequestUserId,
                    OperationSystemVersion = x.OperationSystemVersion,
                    Token = x.Token,
                    UserAgent = x.UserAgent,
                    RequestBody = x.RequestBody
                }).FirstOrDefault();

            return rspns.Succeed(logs);
        }
    }
}
