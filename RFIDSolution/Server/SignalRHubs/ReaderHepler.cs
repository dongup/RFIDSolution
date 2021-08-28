using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.DAL.Shared;
using RFIDSolution.Shared.Models.Shared;
using Symbol.RFID3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Symbol.RFID3.Events;

namespace RFIDSolution.Server.SignalRHubs
{
    public class ReaderHepler
    {
        private AppDbContext _context;
        public ConfigurationEntity SysConfig => _context?.CONFIG.ToList().FirstOrDefault();
        public RFIDReader readerApi;
        public ReadNotifyHandler readNotify;
        private static PostFilter postFilter = null;
        private static AntennaInfo antennaInfo = null;
        private static TriggerInfo triggerInfo = new TriggerInfo();
        public ReaderStatusModel ReaderStatus = new ReaderStatusModel();

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
        }

        /// <summary>
        /// Bắt đầu đọc tag
        /// </summary>
        /// <returns></returns>
        public async Task StartInventory()
        {
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
        public async Task StopInventory()
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
            Console.WriteLine("Connecting reader: " + _context.CONFIG.FirstOrDefault().READER_IP);
            string ip = SysConfig.READER_IP;
            uint port = (uint)SysConfig.READER_PORT;

            triggerInfo.StartTrigger.Type = START_TRIGGER_TYPE.START_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.StopTrigger.Type = STOP_TRIGGER_TYPE.STOP_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.TagReportTrigger = 1;
            triggerInfo.ReportTriggers.Period = (uint)SysConfig.READER_PERIOD;

            if (readerApi == null)
            {
                readerApi = new RFIDReader(ip, port, (uint)SysConfig.READER_TIMEOUT);
            }

            if (!ReaderStatus.IsConnected)
            {
                try
                {
                    //Console.WriteLine("Connecting reader " + ip);
                    readerApi.Connect();
                    ReaderStatus.IsConnected = true;
                    ReaderStatus.Message = "Reader connected at " + ip;
                    Console.WriteLine("Connected reader " + ip);
                }
                catch (Exception ex)
                {
                    ReaderStatus.IsSuccess = false;
                    ReaderStatus.Message = "Connected reader failed, error: "  + ex.InnerException?.Message;
                    Console.WriteLine("Connected reader failed, error: " + ex.InnerException?.Message);
                    return;
                }
            }
            readerApi.Actions.PurgeTags();
            Antennas.Config antennaConfig = readerApi.Config.Antennas[1].GetConfig();
            antennaConfig.TransmitPowerIndex = 132;
            readerApi.Config.Antennas[1].SetConfig(antennaConfig);

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
            readNotify = new ReadNotifyHandler(async (s, e) => await scanDataReceived(e));
            readerApi.Events.ReadNotify += readNotify;
            readerApi.Events.StatusNotify += StatusChanged;
        }

        private void StatusChanged(object sender, StatusEventArgs e)
        {
            OnStatusChanged.Invoke(e);
        }

        public void Disconnect()
        {
            if (ReaderStatus.IsConnected)
            {
                try
                {
                    readerApi.Disconnect();
                    ReaderStatus.IsSuccess = true;
                    ReaderStatus.Message = "Reader disconnected";
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
        private async Task scanDataReceived(ReadEventArgs e)
        {
            try
            {
                if (!ReaderStatus.IsConnected)
                {
                    readerApi.Actions.Inventory.Stop();
                    Console.WriteLine("inventory stoped");
                    return;
                }

                //TagData tagEvent = e.ReadEventData.TagData;
                //sendTag(tagEvent);

                TagData[] tagData = readerApi.Actions.GetReadTags(1000);
                if (tagData == null) return;

                for (int tagIndex = 0; tagIndex < tagData.Length; tagIndex++)
                {
                    TagData tag = tagData[tagIndex];
                    await sendTag(tag);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                readerApi.Actions.Inventory.Stop();
                ReaderStatus.IsInventoring = false;
            }
        }

        /// <summary>
        /// Chuẩn hóa dữ liệu và gửi cho các handler
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public async Task sendTag(TagData tag)
        {
            if (tag == null) return;
            var tagResponse = new RFTagResponse();
            tagResponse.EPCID = tag.TagID;
            tagResponse.RSSI = tag.PeakRSSI;
            tagResponse.SignalStrenght = tag.PeakRSSI + 100;
            tagResponse.LastSeen = DateTime.Now.Ticks;
            tagResponse.AntennaID = tag.AntennaID;
            if (tag.ContainsLocationInfo)
            {
                tagResponse.RelativeDistance = tag.LocationInfo.RelativeDistance;
            }

            //Console.WriteLine($"{tagResponse.EPCID} | {tagResponse.AntennaID} | {tagResponse.LastSeen}");
            OnTagRead.Invoke(tagResponse);
        }

    }
}
