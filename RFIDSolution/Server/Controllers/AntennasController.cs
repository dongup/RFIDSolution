using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RFIDSolution.DataAccess.DAL.Entities;
using RFIDSolution.Server.SignalRHubs;
using RFIDSolution.Shared.DAL;
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
    public class AntennasController : ApiControllerBase
    {
        ReaderHepler _reader => Program.Reader; 

        public AntennasController(AppDbContext context) : base(context)
        {

        }

        [HttpGet]
        public async Task<ResponseModel<List<AntenaModel>>> Get()
        {
            var rspns = new ResponseModel<List<AntenaModel>>();
            var result = _context.ANTENNAS
                .Where(x => true)
                .Select(x => new AntenaModel() { 
                    ANTENNA_ID = x.ANTENNA_ID,
                    ANTENNA_NAME = x.ANTENNA_NAME,
                    ANTENNA_POWER = x.ANTENNA_POWER,
                    LOCATION = x.LOCATION,
                    ID = x.ID
                })
                .ToList();

            return rspns.Succeed(result);
        }

        [HttpGet("powers")]
        public async Task<ResponseModel<List<int>>> GetPowers()
        {
            var rspns = new ResponseModel<List<int>>();
            var result = _reader.TransmitPowerValues;

            return rspns.Succeed(result);
        }


        [HttpGet("availableantennas")]
        public async Task<ResponseModel<List<AntenaModel>>> GetAvailableAntennas()
        {
            var rspns = new ResponseModel<List<AntenaModel>>();
            _reader._context = _context;
            _reader.CheckAntennaStatus();
            var result = _reader.AvailableAntennas;
            return rspns.Succeed(result);
        }

        [HttpPost]
        public async Task<ResponseModel<bool>> Post(AntenaModel value)
        {
            var rspns = new ResponseModel<bool>();
            AntennaEntity newAntena = new AntennaEntity();
            newAntena.ANTENNA_ID = value.ANTENNA_ID;
            newAntena.ANTENNA_NAME = value.ANTENNA_NAME;
            newAntena.ANTENNA_POWER = value.ANTENNA_POWER;
            newAntena.LOCATION = value.LOCATION;
            _context.ANTENNAS.Add(newAntena);
            _context.SaveChanges();
            return rspns.Succeed();
        }

        [HttpPut("{id}")]
        public async Task<ResponseModel<bool>> Put(int id, AntenaModel value)
        {
            var rspns = new ResponseModel<bool>();
            AntennaEntity saveItem = _context.ANTENNAS.Find(id);
            if (saveItem == null) return rspns.NotFound();

            saveItem.ANTENNA_ID = value.ANTENNA_ID;
            saveItem.ANTENNA_NAME = value.ANTENNA_NAME;
            saveItem.ANTENNA_POWER = value.ANTENNA_POWER;
            saveItem.LOCATION = value.LOCATION;
            saveItem.UPDATED_DATE = DateTime.Now;

            _context.SaveChanges();
            return rspns.Succeed();
        }

        [HttpDelete("{id}")]
        public async Task<ResponseModel<bool>> Put(int id)
        {
            var rspns = new ResponseModel<bool>();
            AntennaEntity saveItem = _context.ANTENNAS.Find(id);
            if (saveItem == null) return rspns.NotFound();
            saveItem.IS_DELETED = true;
            saveItem.DELETED_DATE = DateTime.Now;

            _context.SaveChanges();
            return rspns.Succeed();
        }
    }
}
