using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RFIDSolution.DataAccess.DAL.Entities;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Shared;
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
        private readonly IConfiguration configuration;
        public ReaderStatusController(AppDbContext context, IConfiguration configuration) : base(context)
        {
            this.configuration = configuration;
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

        [HttpGet("lightStatus")]
        public ResponseModel<List<LightStatusModel>> GetPort()
        {
            var rspns = new ResponseModel<List<LightStatusModel>>();

            var reader = Program.Reader;
            if (!reader.ReaderStatus.IsConnected) return rspns.Failed("Get light status failed, please connect to reader first!");
            List<LightStatusModel> lightStatuses = reader.GetGPOStatus();
            //List<LightStatusModel> lightStatuses = new List<LightStatusModel>() { 
            //    new LightStatusModel()
            //    {
            //        Type = Shared.Enums.AppEnums.GPOPortType.Green,
            //        PortIndex = 1,
            //        PortState = true,
            //    },
            //    new LightStatusModel()
            //    {
            //        Type = Shared.Enums.AppEnums.GPOPortType.Red,
            //        PortIndex = 2,
            //        PortState = false,
            //    },
            //    new LightStatusModel()
            //    {
            //        Type = Shared.Enums.AppEnums.GPOPortType.Power,
            //        PortIndex = 0,
            //        PortState = true,
            //    },
            //};

            int portRed = configuration.GetSection("RFReaderConfig:RedGPOPort").Get<int>();
            int portGreen = configuration.GetSection("RFReaderConfig:GreenGPOPort").Get<int>();
            int portPower = configuration.GetSection("RFReaderConfig:PowerGPOPort").Get<int>();

            LightStatusModel redLight = lightStatuses.FirstOrDefault(x => x.PortIndex == portRed);
            if(redLight != null)
            {
                redLight.Type = Shared.Enums.AppEnums.GPOPortType.Red;
            }

            LightStatusModel greenLight = lightStatuses.FirstOrDefault(x => x.PortIndex == portGreen);
            if (greenLight != null)
            {
                greenLight.Type = Shared.Enums.AppEnums.GPOPortType.Green;
            }

            //LightStatusModel powerLight = lightStatuses.FirstOrDefault(x => x.PortIndex == portPower);
            //powerLight.Type = Shared.Enums.AppEnums.GPOPortType.Power;

            return rspns.Succeed(lightStatuses);
        }


        [HttpPost("turnOnPort")]
        public ResponseModel<bool> turnOnPort(int port)
        {
            var rspns = new ResponseModel<bool>();
            int portPower = configuration.GetSection("RFReaderConfig:PowerGPOPort").Get<int>();

            if (!Program.Reader.OpenGPOPort(port)) return rspns.Failed("Turn on LED failed! Please connect to reader first!");

            return rspns.Succeed(true);
        }

        [HttpPost("turnOffPort")]
        public ResponseModel<bool> turnOffPort(int port)
        {
            var rspns = new ResponseModel<bool>();

            if (!Program.Reader.ShutDownGPOPort(port)) return rspns.Failed("Turn off LED failed! Please connect to reader first!");

            return rspns.Succeed(true);
        }

    }
}
