using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Shared;
using RFIDSolution.Shared.Models;
using RFIDSolution.Shared.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ApiControllerBase
    {
        public ConfigController(AppDbContext context) :base(context)
        {

        }

        [HttpGet]
        public ResponseModel<ConfigurationEntity> Get()
        {
            var rspns = new ResponseModel<ConfigurationEntity>();
            var config = _context.CONFIG.FirstOrDefault();
            if(config == null)
            {
                config = new ConfigurationEntity();
                config.READER_IP = "192.168.0.111";
                config.READER_PORT = 5084;
                config.READER_PERIOD = 0;
                config.READER_TIMEOUT = 0;
                _context.CONFIG.Add(config);
                _context.SaveChanges();
            }

            return rspns.Succeed(config);
        }

        [HttpPut]
        public ResponseModel<bool> Put(ConfigurationModel value)
        {
            var rspns = new ResponseModel<bool>();
            var config = _context.CONFIG.FirstOrDefault();
            if (config == null)
            {
                config = new ConfigurationEntity();
                config.READER_IP = "192.168.0.111";
                config.READER_PORT = 5084;
                config.READER_PERIOD = 0;
                config.READER_TIMEOUT = 0;
                _context.CONFIG.Add(config);
            }
            else
            {
                config.READER_IP = value.READER_IP;
                config.READER_PORT = value.READER_PORT;
                config.READER_PERIOD = value.READER_PERIOD;
                config.READER_TIMEOUT = value.READER_TIMEOUT;
            }

            _context.SaveChanges();
            return rspns.Succeed();
        }
    }
}
