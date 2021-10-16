using Microsoft.AspNetCore.SignalR;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;
using static Symbol.RFID3.Events;

namespace RFIDSolution.Server.SignalRHubs
{
    public class ReaderStatusHub : Hub
    {
        public ReaderHepler readerApi => Program.Reader;
        public ReaderStatusModel ReaderStatus => readerApi.ReaderStatus;
        private AppDbContext _context;

        public ReaderStatusHub(AppDbContext context)
        {
            _context = context;
        }

        public void CheckStatus()
        {
            var caller = Clients.Caller;
            if (ReaderStatus.IsConnected)
            {
                ReaderStatus.IsSuccess = true;
                ReaderStatus.Message = "Reader connected at " + readerApi.SysConfig.READER_IP;
            }
            else
            {
                ReaderStatus.IsSuccess = false;
                ReaderStatus.Message = "Reader disconnected please try to reconnect!";
            }
            //readerApi._context = _context;
            //readerApi.CheckAntennaStatus();
            ReaderStatus.AvaiableAntennas = readerApi.AvailableAntennas;

            readerApi.OnStatusChanged += (e) => ReaderStatusChanged(caller, e);

            caller.SendAsync("StatusChanged", ReaderStatus);
        }
        
        public void CheckGPIStatus()
        {

        }

        public void CheckAntennaStatus()
        {
            var caller = Clients.Caller;
            if (!readerApi.ReaderStatus.IsConnected) return;
            //readerApi._context = _context;
            readerApi.CheckAntennaStatus();
            ReaderStatus.AvaiableAntennas = readerApi.AvailableAntennas;
            caller.SendAsync("AntennaStatusChanged", ReaderStatus);
        }

        public void ReaderStatusChanged(IClientProxy clientProxy, StatusEventArgs e)
        {
            Console.WriteLine("Status changed " + e.StatusEventData.DisconnectionEventData.DisconnectEventInfo.ToString());
            //readerApi._context = _context;
            //Handle sự kiện reader bị mất kết nối
            if(e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.CONNECTION_LOST
                || e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.READER_INITIATED_DISCONNECTION
                || e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.READER_EXCEPTION)
            {
                clientProxy.SendAsync("StatusChanged", ReaderStatus);
            }
            else if(e.StatusEventData.AntennaEventData.AntennaEvent == Symbol.RFID3.ANTENNA_EVENT_TYPE.ANTENNA_DISCONNECTED)
            { 
                clientProxy.SendAsync("StatusChanged", ReaderStatus);
            }
            else if (e.StatusEventData.AntennaEventData.AntennaEvent == Symbol.RFID3.ANTENNA_EVENT_TYPE.ANTENNA_CONNECTED)
            {
                clientProxy.SendAsync("StatusChanged", ReaderStatus);
            }
        }

        public async Task ConnectReader()
        {
            //readerApi._context = _context;
            //Nếu có người đang cố kết nối trước đó thì chờ trước khi có thể kết nối tiếp nếu không sẽ bị lỗi tranh chấp vùng nhớ
            while (readerApi.connecting)
            {}

            //Nếu đã connect rồi thì không kết nối nữa, nếu không api sẽ báo lỗi
            if (readerApi.ReaderStatus.IsConnected)
            {
                ReaderStatus.Message = "Reader connected at " + readerApi.SysConfig.READER_IP;
                ReaderStatus.IsSuccess = true;
            }
            else
            {
                readerApi.Connect();
                readerApi.StartInventory();
                ReaderStatus.AvaiableAntennas = readerApi.AvailableAntennas;
            }

            //Gửi kết quả cho user
            var caller = Clients.Caller;
            await caller.SendAsync("StatusChanged", ReaderStatus);
        }

        public async Task DisconnectReader()
        {
            //readerApi._context = _context;
            readerApi.Disconnect();
            var caller = Clients.Caller;
            ReaderStatus.AvaiableAntennas.Clear();
            await caller.SendAsync("StatusChanged", ReaderStatus);
        }
    }
}
