using Microsoft.AspNetCore.SignalR;
using RFIDSolution.Shared.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Symbol.RFID3.Events;

namespace RFIDSolution.Server.SignalRHubs
{
    public class ReaderStatusHub : Hub
    {
        public ReaderHepler readerApi => Program.Reader;
        public ReaderStatusModel ReaderStatus => readerApi.ReaderStatus;

        public ReaderStatusHub()
        {

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
            caller.SendAsync("StatusChanged", ReaderStatus);
            readerApi.OnStatusChanged += (e) => ReaderStatusChanged(caller, e);
        }
        
        public void CheckGPIStatus()
        {

        }

        public void ReaderStatusChanged(IClientProxy clientProxy, StatusEventArgs e)
        {
            Console.WriteLine("Status changed " + e.StatusEventData.DisconnectionEventData.DisconnectEventInfo.ToString());
            //Handle sự kiện reader bị mất kết nối
            if(e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.CONNECTION_LOST)
            {
                readerApi.ReaderStatus.IsConnected = false;
                readerApi.ReaderStatus.Message = "Reader connection lost!";
                Console.WriteLine("Reader connection lost");
                clientProxy.SendAsync("StatusChanged", ReaderStatus);
            }
        }

        public void ConnectReader()
        {
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
            }

            //Gửi kết quả cho user
            var caller = Clients.Caller;
            caller.SendAsync("StatusChanged", ReaderStatus);
        }

        public void DisconnectReader()
        {
            readerApi.Disconnect();
            var caller = Clients.Caller;
            caller.SendAsync("StatusChanged", ReaderStatus);
        }
    }
}
