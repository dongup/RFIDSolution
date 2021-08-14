using RFIDSolution.Shared.Protos;
using RFIDSolution.WebAdmin.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Server.Service
{
    public class RFIDReadService : RFTagProto.RFTagProtoBase
    {
        private AppDbContext _context;

        public RFIDReadService(AppDbContext context)
        {
            _context = context;
        }

        public override async Task Get(RFTagFilter request,
            Grpc.Core.IServerStreamWriter<RFTagResponse> responseStream,
            Grpc.Core.ServerCallContext context)
        {
            bool reading = true;
            Console.WriteLine("Start reading anten " + request.AntenId);
            var tags = new List<string>() {
                 Guid.NewGuid().ToString(),
                 Guid.NewGuid().ToString(),
                 Guid.NewGuid().ToString(),
            };
            var rand = new Random();

            while (reading)
            {
                var response = new RFTagResponse();
                var index = rand.Next(0, 2);
                var strenght = rand.Next(0, 30);
                response.EPCID = tags[index];
                response.LastSeen = DateTime.Now.Ticks;
                response.RSSI = strenght;

                await Task.Delay(1);
                try
                {
                    await responseStream.WriteAsync(response);
                }
                catch
                {
                    Console.WriteLine("Stop reading anten " + request.AntenId);
                    reading = false;
                }
            }
        }
    }
}
