using Grpc.Core;
using RFIDSolution.Shared.DAL;
using RFIDSolution.Shared.Protos;
using Symbol.RFID3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Symbol.RFID3.Events;

namespace RFIDSolution.Server.Service
{
    public class RFIDReadService : RFTagProto.RFTagProtoBase
    {
        private AppDbContext _context;
        public static RFIDReader readerApi;
        private static IServerStreamWriter<RFTagResponse> _clientSteam;
        private static ServerCallContext _callContext;
        private static bool reading = false;
        private static bool connected = false;
        private static TagData[] tagData;

        public RFIDReadService(AppDbContext context)
        {
            _context = context;
        }

        public override async Task GetScanData(RFTagFilter request,
            Grpc.Core.IServerStreamWriter<RFTagResponse> responseStream,
            Grpc.Core.ServerCallContext context)
        {
            _clientSteam = responseStream;
            _callContext = context;
            string ip = "192.168.0.111";
            uint port = 5084;
           

            PostFilter postFilter = null;
            AntennaInfo antennaInfo = null;
            TriggerInfo triggerInfo = new TriggerInfo();
            triggerInfo.StartTrigger.Type = START_TRIGGER_TYPE.START_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.StopTrigger.Type = STOP_TRIGGER_TYPE.STOP_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.TagReportTrigger = 1;
            triggerInfo.ReportTriggers.Period = 0;


            if(readerApi == null)
            {
                readerApi = new RFIDReader(ip, port, 0);
                if (!connected)
                {
                    readerApi.Connect();
                    connected = true;
                    Console.WriteLine("Connected reader " + ip);
                }
                if (readerApi.ReaderCapabilities.IsTagEventReportingSupported)
                {
                    triggerInfo.TagEventReportInfo.ReportNewTagEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                    triggerInfo.TagEventReportInfo.ReportTagBackToVisibilityEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                    triggerInfo.TagEventReportInfo.ReportTagInvisibleEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                    triggerInfo.TagEventReportInfo.NewTagEventModeratedTimeoutMilliseconds = 500;
                    triggerInfo.TagEventReportInfo.TagBackToVisibilityModeratedTimeoutMilliseconds = 500;
                    triggerInfo.TagEventReportInfo.TagInvisibleEventModeratedTimeoutMilliseconds = 500;
                }

                readerApi.Events.ReadNotify += new ReadNotifyHandler(scanDataReceived);

            }

            Console.WriteLine("EPC | AntenId | Last seen");
            reading = true;
            readerApi.Actions.Inventory.Perform(
                   postFilter,
                   triggerInfo,
                   antennaInfo);

            while (!_callContext.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(10);
                try
                {
                    //TagData tagEvent = e.ReadEventData.TagData;
                    //sendTag(tagEvent);
                    Console.WriteLine("Reading...");
                    //TagData[] tagData = readerApi.Actions.GetReadTags(1000);
                    if (tagData == null) continue;

                    for (int tagIndex = 0; tagIndex < tagData.Length; tagIndex++)
                    {
                        TagData tag = tagData[tagIndex];
                        sendTag(tag);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.StackTrace);

                    readerApi.Actions.Inventory.Stop();
                }
            }
            reading = false;
            readerApi.Actions.Inventory.Stop();
            Console.WriteLine("inventory stoped");
        }

        private static void scanDataReceived(object sender, ReadEventArgs e)
        {
            try
            {
                tagData = readerApi.Actions.GetReadTags(1000);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //try
            //{
            //    Console.WriteLine("Scan received canceled: " + _callContext.CancellationToken.IsCancellationRequested);
            //    if (_callContext.CancellationToken.IsCancellationRequested)
            //    {
            //        readerApi.Actions.Inventory.Stop();
            //        Console.WriteLine("inventory stoped");
            //        return;
            //    }

            //    //TagData tagEvent = e.ReadEventData.TagData;
            //    //sendTag(tagEvent);

            //    TagData[] tagData = readerApi.Actions.GetReadTags(1000);
            //    if (tagData == null) return;

            //    for (int tagIndex = 0; tagIndex < tagData.Length; tagIndex++)
            //    {
            //        TagData tag = tagData[tagIndex];
            //        sendTag(tag);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.StackTrace);
            //    Console.WriteLine(ex.Message);
            //    Console.WriteLine(ex.InnerException?.StackTrace);

            //    readerApi.Actions.Inventory.Stop();
            //}
        }

        public static void sendTag(TagData tag)
        {
            if (tag == null) return;
            Console.WriteLine($"{tag.TagID} | {tag.AntennaID}");
            var tagResponse = new RFTagResponse();
            tagResponse.EPCID = tag.TagID;
            tagResponse.RSSI = tag.PeakRSSI + 100;
            tagResponse.LastSeen = DateTime.Now.Ticks;

            _clientSteam.WriteAsync(tagResponse);
        }

        public override async Task GetDemoMapping(RFTagFilter request,
            Grpc.Core.IServerStreamWriter<RFTagResponse> responseStream,
            Grpc.Core.ServerCallContext context)
        {
            bool reading = true;
            //Console.WriteLine("Start reading anten " + request.AntenId);
            var tags = new List<string>() {
                 Guid.NewGuid().ToString(),
                 Guid.NewGuid().ToString(),
                 Guid.NewGuid().ToString(),
            };
            var rand = new Random();

            while (reading)
            {
                var response = new RFTagResponse();
                var index = rand.Next(0, 1);
                var strenght = rand.Next(70, 100);
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
                    //Console.WriteLine("Stop reading anten " + request.AntenId);
                    reading = false;
                }
            }
        }

        public override async Task GetDataDemo(RFTagFilter request,
            Grpc.Core.IServerStreamWriter<RFTagResponse> responseStream,
            Grpc.Core.ServerCallContext context)
        {
            bool reading = true;
            //Console.WriteLine("Start reading anten " + request.AntenId);
            var tag = _context.PRODUCT.OrderBy(x => Guid.NewGuid()).FirstOrDefault().EPC;
            var rand = new Random();
            Console.WriteLine("sending tag: " + tag);

            while (reading)
            {
                var response = new RFTagResponse();
                response.EPCID = tag;
                response.LastSeen = DateTime.Now.Ticks;

                var strenght = rand.Next(70, 100);
                response.RSSI = strenght;

                await Task.Delay(1);
                try
                {
                    await responseStream.WriteAsync(response);
                }
                catch
                {
                    reading = false;
                }
            }
        }
    }
}
