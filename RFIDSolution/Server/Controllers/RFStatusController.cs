using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.DataAccess.DAL.Entities;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaiyoshaEPE.WebApi.Hubs;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReaderStatusController : ApiControllerBase
    {
        public ReaderStatusController(AppDbContext context) : base(context)
        {

        }

        [HttpGet("clients")]
        public ResponseModel<List<string>> GetClients()
        {
            var rspns = new ResponseModel<List<string>>();
            var clients = ReaderHub.RFClients.Select(x => x.Id).ToList();
            return rspns.Succeed(clients);
        }

        [HttpGet("events")]
        public ResponseModel<PaginationResponse<ReaderLogEntity>> GetEvents(int eventType = 0, int pageItem = 10, int pageIndex = 0)
        {
            var rspns = new ResponseModel<PaginationResponse<ReaderLogEntity>>();
            var result = _context.READER_LOGS
                .Where(x => (eventType == 0 || (int)x.LOG_TYPE == eventType))
                .OrderByDescending(x => x.CREATED_DATE)
                .AsQueryable();

            return rspns.Succeed(new PaginationResponse<ReaderLogEntity>(result, pageItem, pageIndex));
        }

        [HttpPost("turnOnPort")]
        public ResponseModel<bool> turnOnPort(int port)
        {
            var rspns = new ResponseModel<bool>();

            Program.Reader.OpenGPOPort(port);

            return rspns.Succeed(true);
        }

        [HttpPost("turnOffPort")]
        public ResponseModel<bool> turnOffPort(int port)
        {
            var rspns = new ResponseModel<bool>();

            Program.Reader.ShutDownGPOPort(port);

            return rspns.Succeed(true);
        }

    }
}
