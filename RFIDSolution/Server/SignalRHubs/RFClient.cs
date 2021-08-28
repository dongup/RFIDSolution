using Microsoft.AspNetCore.SignalR;
using RFIDSolution.Shared.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Server.SignalRHubs.ReaderHepler;

namespace RFIDSolution.Server.SignalRHubs
{
    public class RFClient
    {
        public RFClient()
        {

        }

        public string Id { get; set; }

        public IClientProxy ClientProxy { get; set; }

        public RFTagRequest TagRequest { get; set; }

        public TagReadHandler ReadHandler { get; set; }
    }
}
