using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class RFStatusController : ControllerBase
    {
        public RFStatusController()
        {

        }

        [HttpGet("clients")]
        public ResponseModel<List<string>> GetClients()
        {
            var rspns = new ResponseModel<List<string>>();
            var clients = ReaderHub.RFClients.Select(x => x.Id).ToList();
            return rspns.Succeed(clients);
        }
    }
}
