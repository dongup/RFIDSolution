using Microsoft.AspNetCore.SignalR;
using RFIDSolution.Shared.DAL;
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
            readerApi._context = _context;
            readerApi.CheckAntennaStatus();
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
            readerApi._context = _context;
            readerApi.CheckAntennaStatus();
            ReaderStatus.AvaiableAntennas = readerApi.AvailableAntennas;

            caller.SendAsync("AntennaStatusChanged", ReaderStatus);
        }

        public void ReaderStatusChanged(IClientProxy clientProxy, StatusEventArgs e)
        {
            Console.WriteLine("Status changed " + e.StatusEventData.DisconnectionEventData.DisconnectEventInfo.ToString());
            readerApi._context = _context;
            //Handle sự kiện reader bị mất kết nối
            if(e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.CONNECTION_LOST
                || e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.READER_INITIATED_DISCONNECTION
                || e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.READER_EXCEPTION)
            {
                readerApi.ReaderStatus.IsConnected = false;
                readerApi.ReaderStatus.Message = "Reader connection lost!";
                Console.WriteLine("Reader connection lost");
                clientProxy.SendAsync("StatusChanged", ReaderStatus);
            }
            else if(e.StatusEventData.AntennaEventData.AntennaEvent == Symbol.RFID3.ANTENNA_EVENT_TYPE.ANTENNA_DISCONNECTED)
            {
                int antennaId = e.StatusEventData.AntennaEventData.AntennaID;

                Console.WriteLine($"Antenna {antennaId} disconnected");
                readerApi.ReaderStatus.Message = $"Antenna {e.StatusEventData.AntennaEventData.AntennaID} disconnected";

                //readerApi.CheckAntennaStatus();
                var anten = readerApi.AvailableAntennas.FirstOrDefault(x => x.ANTENNA_ID == antennaId);
                if(anten != null)
                {
                    anten.ANTENNA_STATUS = Shared.Enums.AppEnums.AntennaStatus.Disconnected;
                    readerApi.ReaderStatus.AvaiableAntennas = readerApi.AvailableAntennas;
                    Console.WriteLine("Sending status to client");
                    clientProxy.SendAsync("StatusChanged", ReaderStatus);
                }
            }
            else if (e.StatusEventData.AntennaEventData.AntennaEvent == Symbol.RFID3.ANTENNA_EVENT_TYPE.ANTENNA_CONNECTED)
            {
                int antennaId = e.StatusEventData.AntennaEventData.AntennaID;

                Console.WriteLine($"Antenna {antennaId} connected");
                readerApi.ReaderStatus.Message = $"Antenna {e.StatusEventData.AntennaEventData.AntennaID} connected";

                readerApi.CheckAntennaStatus();
                readerApi.ReaderStatus.AvaiableAntennas = readerApi.AvailableAntennas;
                clientProxy.SendAsync("StatusChanged", ReaderStatus);
            }
        }

        public void ConnectReader()
        {
            readerApi._context = _context;
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
            readerApi._context = _context;
            readerApi.Disconnect();
            var caller = Clients.Caller;
            caller.SendAsync("StatusChanged", ReaderStatus);
        }
    }
}
