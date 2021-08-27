using Microsoft.AspNetCore.SignalR;
using RFIDSolution.Shared.Models.Shared;
using Symbol.RFID3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Symbol.RFID3.Events;

namespace TaiyoshaEPE.WebApi.Hubs
{
    public class ReaderHub : Hub
    {
        public static RFIDReader readerApi;
        private static bool connected = false;

        public async Task StartScan(RFTagRequest request)
        {
            string ip = "192.168.0.111";
            uint port = 5084;

            PostFilter postFilter = null;
            AntennaInfo antennaInfo = null;
            TriggerInfo triggerInfo = new TriggerInfo();
            triggerInfo.StartTrigger.Type = START_TRIGGER_TYPE.START_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.StopTrigger.Type = STOP_TRIGGER_TYPE.STOP_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.TagReportTrigger = 1;
            triggerInfo.ReportTriggers.Period = 0;


            if (readerApi == null)
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

                readerApi.Events.ReadNotify += async (s, e) => await scanDataReceived(Clients.Caller, e);

            }

            //Console.WriteLine("EPC | AntenId | Last seen");
            readerApi.Actions.Inventory.Perform(
                   postFilter,
                   triggerInfo,
                   antennaInfo);
        }

        private async Task scanDataReceived(IClientProxy client, ReadEventArgs e)
        {
            try
            {
                if (!connected)
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
                    await sendTag(tag, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                readerApi.Actions.Inventory.Stop();
            }
        }

        public static async Task sendTag(TagData tag, IClientProxy client)
        {
            if (tag == null) return;
            Console.WriteLine($"{tag.TagID} | {tag.AntennaID}");
            var tagResponse = new RFTagResponse();
            tagResponse.EPCID = tag.TagID;
            tagResponse.RSSI = tag.PeakRSSI + 100;
            tagResponse.LastSeen = DateTime.Now.Ticks;

            await client.SendAsync("ReceiveTag", tag);
        }
    }
}
