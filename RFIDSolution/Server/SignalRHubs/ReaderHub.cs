﻿using Microsoft.AspNetCore.SignalR;
using RFIDSolution.Server;
using RFIDSolution.Server.SignalRHubs;
using RFIDSolution.Shared.DAL;
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
        public ReaderHepler ReaderApi => Program.Reader;

        public static List<RFClient> RFClients { get => rFClients; set => rFClients = value; }

        public AppDbContext _context;

        public ReaderHub(AppDbContext context) 
        {
            _context = context;
        }

        private static readonly List<RFClient> rfClients = new List<RFClient>();

        /// <summary>
        /// List các thiết bị đang đọc tín hiệu
        /// </summary>
        private static List<RFClient> rFClients = rfClients;

        /// <summary>
        /// Thêm handler cho sự kiện đọc tag khi một client bắt đầu đọc
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task StartInventory(RFTagRequest request)
        {
            //Code chỉ dùng khi không có reader
            var mockData = await GetRandomEPC(3);
            foreach (string EPC in mockData)
            {
                RFTagResponse tag = new RFTagResponse();
                tag.EPCID = EPC;
                tag.LastSeen = DateTime.Now.Ticks;
                tag.RSSI = -32;

                var caller = Clients.Caller;
                await caller.SendAsync("ReceiveTag", tag);
            }
            return;

            if (!ReaderApi.ReaderStatus.IsConnected)
            {
                await Clients.Caller.SendAsync("OnError", "Reader is not connected!");
                return;
            }

            var client = RFClients.FirstOrDefault(x => x.Id == Context.ConnectionId);
            //Nếu là người mới thì thêm vào danh sách
            if (client == null)
            {
                client = new RFClient();
                client.Id = Context.ConnectionId;
                client.ReadHandler = new TagReadHandler(async (e) => await OnTagRead(e, client));
                client.TagRequest = request;
                client.ClientProxy = Clients.Caller;
                RFClients.Add(client);
            }

            //Code sử dụng khi có reader
            ReaderApi.OnTagRead += client.ReadHandler;
            
        }

        /// <summary>
        /// Lấy ra ngẫu nhiên 1 lượng EPC đã có trong hệ thống
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<List<string>> GetRandomEPC(int number)
        {
            var query = _context.PRODUCT
                .OrderByDescending(x => Guid.NewGuid())
                .Take(number)
                .Where(x => !string.IsNullOrEmpty(x.EPC))
                .Select(x => x.EPC)
                .ToList();

            return query;
        }


        public void StopInventory()
        {
            var client = RFClients.FirstOrDefault(x => x.Id == Context.ConnectionId);
            Console.WriteLine("Client stop Id: " + Context.ConnectionId);
            if (client != null)
            {
                ReaderApi.OnTagRead -= client.ReadHandler;
            }
        }

        private async Task OnTagRead(RFTagResponse tag, RFClient client)
        {
            //Nếu client đã disconnect thì bỏ handler ngùng send data
            if(!RFClients.Any(x => x.Id == client.Id))
            {
                ReaderApi.OnTagRead -= client.ReadHandler;

                //Chỉ sử dụng khi không có reader
                //readerApi.OnTagRead = null;
                return;
            }    

            if (client == null) return;
            //Console.WriteLine("Sending tags " + client.TagRequest.AntenIds.FirstOrDefault());
            if (client.TagRequest.AntenIds.Contains(tag.AntennaID))
            {
                await client.ClientProxy.SendAsync("ReceiveTag", tag);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine(String.Format("Client {0} explicitly closed the connection.", Context.ConnectionId));
            if (Context != null) return;
            var client = RFClients.FirstOrDefault(x => x.Id == Context.ConnectionId);
            if (client != null)
            {
                ReaderApi.OnTagRead -= client.ReadHandler;
                RFClients.Remove(client);
            }
        }
    }
}
