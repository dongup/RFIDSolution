using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RFIDSolution.DataAccess.DAL.Entities;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Shared;
using RFIDSolution.Shared.Models.Shared;
using RFIDSolution.Shared.Utils;
using Symbol.RFID3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;
using static Symbol.RFID3.Events;

namespace RFIDSolution.Server.SignalRHubs
{
    public class ReaderHepler
    {
        public AppDbContext _context;
        public ConfigurationEntity SysConfig = new ConfigurationEntity();
        public RFIDReader readerApi;
        public ReadNotifyHandler readNotify;
        private static readonly PostFilter postFilter = null;
        private static readonly AntennaInfo antennaInfo = null;
        private static readonly TriggerInfo triggerInfo = new TriggerInfo();
        public ReaderStatusModel ReaderStatus = new ReaderStatusModel();
        public bool connecting = false;

        public List<int> TransmitPowerValues = new List<int>();
        public List<AntennaEntity> Antennas = new List<AntennaEntity>();
        public List<AntenaModel> AvailableAntennas = new List<AntenaModel>();


        //Handler sự kiện đọc tag
        public delegate void TagReadHandler(RFTagResponse tag);
        public TagReadHandler OnTagRead = null;

        //Handler sự kiện reader disconnected
        public delegate void StatusChangedHandler(StatusEventArgs tag);
        public StatusChangedHandler OnStatusChanged = null;

        public ReaderHepler()
        {

        }

        public ReaderHepler(AppDbContext context)
        {
            _context = context;
            SysConfig = _context.CONFIG.ToList().FirstOrDefault();
            Antennas = _context.ANTENNAS.AsNoTracking().ToList();
        }

        /// <summary>
        /// Bắt đầu đọc tag
        /// </summary>
        /// <returns></returns>
        public void StartInventory()
        {
            Console.WriteLine("Starting inventory process...");
            //Chờ tới khi stop xong mới start cái mới
            while (ReaderStatus.IsInventoring) { }
            if (!ReaderStatus.IsConnected || readerApi == null)
            {
                Connect();
            }
            readerApi.Actions.PurgeTags();
            readerApi.Actions.Inventory.Perform(
               postFilter,
               triggerInfo,
               antennaInfo);
            ReaderStatus.IsInventoring = true;
            Console.WriteLine("Inventory started");
        }

        /// <summary>
        /// Kết thúc đọc tag
        /// </summary>
        /// <returns></returns>
        public void StopInventory()
        {
            readerApi.Actions.Inventory.Stop();
            ReaderStatus.IsInventoring = false;
            Console.WriteLine("Inventory stopped");
        }

        /// <summary>
        /// Kết nối reader, set config anten
        /// </summary>
        public void Connect()
        {
            connecting = true;
            _context.Entry(SysConfig).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            try
            {
                SysConfig = _context.CONFIG.ToList().FirstOrDefault();
            }
            catch 
            {
            }

            Console.WriteLine("Connecting reader: " + SysConfig.READER_IP);
            string ip = SysConfig.READER_IP;
            uint port = (uint)SysConfig.READER_PORT;

            triggerInfo.StartTrigger.Type = START_TRIGGER_TYPE.START_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.StopTrigger.Type = STOP_TRIGGER_TYPE.STOP_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.TagReportTrigger = 1;
            triggerInfo.ReportTriggers.Period = (uint)SysConfig.READER_PERIOD;

            readerApi = null;
            readerApi = new RFIDReader(ip, port, (uint)SysConfig.READER_TIMEOUT);

            if (!ReaderStatus.IsConnected)
            {
                try
                {
                    //Console.WriteLine("Connecting reader " + ip);
                    readerApi.Connect();
                    ReaderStatus.IsConnected = true;
                    ReaderStatus.IsSuccess = true;
                    ReaderStatus.Message = "Reader connected at " + ip;

                    LogReaderEvent("Reader connected at " + ip, RdrLog.Connect);
                    Console.WriteLine("Connected reader " + ip);
                    connecting = false;
                }
                catch (Exception ex)
                {
                    ReaderStatus.IsSuccess = false;
                    ReaderStatus.IsConnected = false;
                    ReaderStatus.Message = "Reader connection failed, please try again!";
                    Console.WriteLine("Connected reader failed, error: " + ex.InnerException?.Message);
                    LogReaderEvent("Reader connect failed", RdrLog.Error);
                    connecting = false;
                    return;
                }
                connecting = false;
            }
            readerApi.Actions.PurgeTags();

            readerApi.Events.NotifyAntennaEvent = true;
            readerApi.Events.NotifyReaderDisconnectEvent = true;
            readerApi.Events.NotifyReaderExceptionEvent = true;

            CheckAntennaStatus();

            if (readerApi.ReaderCapabilities.IsTagEventReportingSupported)
            {
                triggerInfo.TagEventReportInfo.ReportNewTagEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                triggerInfo.TagEventReportInfo.ReportTagBackToVisibilityEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                triggerInfo.TagEventReportInfo.ReportTagInvisibleEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                triggerInfo.TagEventReportInfo.NewTagEventModeratedTimeoutMilliseconds = 500;
                triggerInfo.TagEventReportInfo.TagBackToVisibilityModeratedTimeoutMilliseconds = 500;
                triggerInfo.TagEventReportInfo.TagInvisibleEventModeratedTimeoutMilliseconds = 500;
            }
            if (readNotify != null)
            {
                readerApi.Events.ReadNotify -= readNotify;
            }
            readNotify = new ReadNotifyHandler((s, e) =>
            {
                ScanDataReceived(e);
            });
            readerApi.Events.ReadNotify += readNotify;
            readerApi.Events.StatusNotify += StatusChanged;
        }

        public void ReConnect()
        {
            readerApi.Reconnect();
        }

        public List<LightStatusModel> GetGPOStatus()
        {
            //int numGPOPort = readerApi.ReaderCapabilities.NumGPOPorts;
            List<LightStatusModel> lightStatuses = new List<LightStatusModel>();

            for(int i = 1; i <= 2; i++)
            {
                var gpo = readerApi.Config.GPO[i];
                LightStatusModel lightStatus = new LightStatusModel();
                lightStatus.PortIndex = i;
                lightStatus.PortState = gpo.PortState == GPOs.GPO_PORT_STATE.TRUE;
                lightStatuses.Add(lightStatus);
            }

            return lightStatuses;
        }

        public void CheckAntennaStatus()
        {
            AvailableAntennas.Clear();
            if (!ReaderStatus.IsConnected) return;
            //Lay thong so power
            TransmitPowerValues = readerApi.ReaderCapabilities.TransmitPowerLevelValues.ToList();

            //Lấy thông tin anntena
            int antenCount = readerApi.Config.Antennas.AvailableAntennas.Length;
            for (int antenIndex = 1; antenIndex <= antenCount; antenIndex++)
            {
                AntenaModel antena = new AntenaModel();
                var antennaInfor = readerApi.Config.Antennas[antenIndex];
                antena.ANTENNA_ID = antennaInfor.Index;
                antena.ANTENNA_STATUS = antennaInfor.GetPhysicalProperties().IsConnected ? Shared.Enums.AppEnums.AntennaStatus.Connected : Shared.Enums.AppEnums.AntennaStatus.Disconnected;
                Console.WriteLine("checking antenna: " + antena.ANTENNA_ID);

                //Tìm thông tin config của id antena đã lưu và set config cho reader
                var savedAntenna = Antennas.FirstOrDefault(x => x.ANTENNA_ID == antena.ANTENNA_ID);
                if(savedAntenna != null)
                {
                    Antennas.Config antennaConfig = antennaInfor.GetConfig();
                    int powerIndex = TransmitPowerValues.IndexOf(savedAntenna.ANTENNA_POWER);
                    //Console.WriteLine("Power: " + savedAntenna.ANTENNA_POWER);
                    if(powerIndex != -1)
                    {
                        Console.WriteLine("Antenna Power found: " + powerIndex);
                        antennaConfig.TransmitPowerIndex = (ushort)powerIndex;
                        antennaInfor.SetConfig(antennaConfig);
                    }
                    //Nếu không tìm thấy power index thì set power cao nhất có thể và update lại config cho hợp lệ
                    else
                    {
                        Console.WriteLine("Antenna Power not found setting max power: ");
                        antennaConfig.TransmitPowerIndex = (ushort)(TransmitPowerValues.Count - 1);
                        antennaInfor.SetConfig(antennaConfig);
                        savedAntenna.ANTENNA_POWER = TransmitPowerValues[antennaConfig.TransmitPowerIndex];
                        _context.SaveChanges();
                    }
                }
                else
                {
                    antena.ANTENNA_STATUS = AntennaStatus.Unknown;
                }
                AvailableAntennas.Add(antena);
            }
        }

        private void StatusChanged(object sender, StatusEventArgs e)
        {
           //Handle sự kiện reader bị mất kết nối
            if(e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.CONNECTION_LOST
                || e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.READER_INITIATED_DISCONNECTION
                || e.StatusEventData.DisconnectionEventData.DisconnectEventInfo == Symbol.RFID3.DISCONNECTION_EVENT_TYPE.READER_EXCEPTION)
            {
                ReaderStatus.IsConnected = false;
                ReaderStatus.Message = "Reader connection lost!";
                Console.WriteLine("Reader connection lost");

                //Gửi mail và ghi log ở 1 thread khác
                LogReaderEvent("Reader disconnected due to connection issue", RdrLog.Disconnect);

            }
            else if(e.StatusEventData.AntennaEventData.AntennaEvent == Symbol.RFID3.ANTENNA_EVENT_TYPE.ANTENNA_DISCONNECTED)
            {
                int antennaId = e.StatusEventData.AntennaEventData.AntennaID;

                Console.WriteLine($"Antenna {antennaId} disconnected");
                ReaderStatus.Message = $"Antenna {e.StatusEventData.AntennaEventData.AntennaID} disconnected";

                //CheckAntennaStatus();
                var anten = AvailableAntennas.FirstOrDefault(x => x.ANTENNA_ID == antennaId);
                if(anten != null)
                {
                    anten.ANTENNA_STATUS = Shared.Enums.AppEnums.AntennaStatus.Disconnected;
                    ReaderStatus.AvaiableAntennas = AvailableAntennas;
                    //Console.WriteLine("Sending status to client");
                }
            }
            else if (e.StatusEventData.AntennaEventData.AntennaEvent == Symbol.RFID3.ANTENNA_EVENT_TYPE.ANTENNA_CONNECTED)
            {
                int antennaId = e.StatusEventData.AntennaEventData.AntennaID;

                Console.WriteLine($"Antenna {antennaId} connected");
                ReaderStatus.Message = $"Antenna {e.StatusEventData.AntennaEventData.AntennaID} connected";

                CheckAntennaStatus();
                ReaderStatus.AvaiableAntennas = AvailableAntennas;
            }

            OnStatusChanged.Invoke(e);
        }

        public bool OpenGPOPort(int port)
        {
            if (!ReaderStatus.IsConnected) return false;
            Console.WriteLine($"[{DateTime.Now.ToVNString()}] Opening port " + port);

            readerApi.Config.GPO[port].PortState = GPOs.GPO_PORT_STATE.TRUE;
            var delay = SysConfig.GPO_RESET_TIME;
            if (delay != 0)
            {
                _ = Task.Run(async () => {
                    await Task.Delay(delay);
                    readerApi.Config.GPO[port].PortState = GPOs.GPO_PORT_STATE.FALSE;
                });
            }
            return true;
        }

        public bool ShutDownGPOPort(int port)
        {
            if (!ReaderStatus.IsConnected) return false;

            readerApi.Config.GPO[port].PortState = GPOs.GPO_PORT_STATE.FALSE;
            return true;
        }

        public void Disconnect()
        {
            if (ReaderStatus.IsConnected)
            {
                try
                {
                    Console.WriteLine("Disconnecting reader");
                    if (ReaderStatus.IsInventoring)
                    {
                        readerApi.Actions.Inventory.Stop();
                    }
                    readerApi.Disconnect();
                    ReaderStatus.IsConnected = false;
                    ReaderStatus.IsInventoring = false;
                    ReaderStatus.IsSuccess = true;
                    ReaderStatus.Message = "Reader disconnected";
                    string userName = "Admin";
                    LogReaderEvent($"User {userName} explicitly disconnect the reader", RdrLog.Disconnect);
                    Console.WriteLine("Reader disconnected");
                }
                catch (Exception ex)
                {
                    ReaderStatus.IsSuccess = false;
                    ReaderStatus.Message = "Reader disconnect failed, error: " + ex.Message;
                }
            }
        }

        /// <summary>
        /// Lấy dữ liệu thô từ reader
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private void ScanDataReceived(ReadEventArgs e)
        {
            try
            {
                if (readerApi == null) return;

                if (!ReaderStatus.IsConnected)
                {
                    readerApi.Actions.Inventory.Stop();
                    //Console.WriteLine("inventory stoped");
                    return;
                }

                //TagData tagEvent = e.ReadEventData.TagData;
                //await sendTag(tagEvent);

                TagData[] tagData = readerApi.Actions.GetReadTags(1000);
                if (tagData == null) return;

                for (int tagIndex = 0; tagIndex < tagData.Length; tagIndex++)
                {
                    TagData tag = tagData[tagIndex];
                    SendTag(tag);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                readerApi.Actions.Inventory.Stop();
                ReaderStatus.IsInventoring = false;
            }
        }

        /// <summary>
        /// Chuẩn hóa dữ liệu và gửi cho các handler
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public void SendTag(TagData tag)
        {
            if (tag == null) return;
            var tagResponse = new RFTagResponse
            {
                EPCID = tag.TagID,
                RSSI = tag.PeakRSSI,
                SignalStrenght = tag.PeakRSSI + 100 > 100 ? 100 : tag.PeakRSSI + 100 < 0 ? 0 : tag.PeakRSSI + 100,
                LastSeen = DateTime.Now.Ticks,
                AntennaID = tag.AntennaID
            };
            if (tag.ContainsLocationInfo)
            {
                tagResponse.RelativeDistance = tag.LocationInfo.RelativeDistance;
            }

            //Console.WriteLine($"{tagResponse.EPCID} | {tagResponse.AntennaID} | {tagResponse.LastSeen}");
            OnTagRead.Invoke(tagResponse);
        }

        public void LogReaderEvent(string content, RdrLog type)
        {
            try {
                ReaderLogEntity newLog = new ReaderLogEntity
                {
                    LOG_CONTENT = content,
                    LOG_TYPE = type,
                    CREATED_DATE = DateTime.Now,
                    NOTE = ""
                };

                _context.READER_LOGS.Add(newLog);
                _context.SaveChangesAsync();

                //Gửi mail

            }catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
            } 
        }
    }
}
