using Microsoft.AspNetCore.SignalR;
using RFIDSolution.Server;
using RFIDSolution.Server.SignalRHubs;
using RFIDSolution.Shared.Models.Shared;
using Symbol.RFID3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Server.SignalRHubs.ReaderHepler;
using static Symbol.RFID3.Events;

namespace TaiyoshaEPE.WebApi.Hubs
{
    public class ReaderHub : Hub
    {
        public ReaderHepler readerApi => Program.Reader;
        /// <summary>
        /// List các thiết bị đang đọc tín hiệu
        /// </summary>
        public static List<RFClient> RFClients = new List<RFClient>();

        /// <summary>
        /// Thêm handler cho sự kiện đọc tag khi một client bắt đầu đọc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task StartInventory(RFTagRequest request)
        {
            var client = RFClients.FirstOrDefault(x => x.Id == Context.ConnectionId);
            //Nếu là người mới thì thêm vào danh sách
            if(client == null)
            {
                client = new RFClient();
                client.Id = Context.ConnectionId;
                client.ReadHandler = new TagReadHandler(async (e) => await OnTagRead(e, client));
                client.TagRequest = request;
                client.ClientProxy = Clients.Caller;
                RFClients.Add(client);
            }
            readerApi.OnTagRead += client.ReadHandler;
        }

        public async Task StopInventory()
        {
            var client = RFClients.FirstOrDefault(x => x.Id == Context.ConnectionId);
            Console.WriteLine("Connection Id: " + Context.ConnectionId);
            if (client != null)
            {
                readerApi.OnTagRead -= client.ReadHandler;
            }
        }

        private async Task OnTagRead(RFTagResponse tag, RFClient client)
        {
            //Console.WriteLine("Tag antennaId: " + tag.AntennaID);
            if (client == null) return;
            if (tag.AntennaID == client.TagRequest.AntenId)
            {
                Console.WriteLine($"[{DateTime.Now.Ticks}] Sending to: " + client.Id);
                client.ClientProxy.SendAsync("ReceiveTag", tag);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
            var client = RFClients.FirstOrDefault(x => x.Id == Context.ConnectionId);
            if (client != null)
            {
                readerApi.OnTagRead -= client.ReadHandler;
                RFClients.Remove(client);
            }
        }
    }
}
