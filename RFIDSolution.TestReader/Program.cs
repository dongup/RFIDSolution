using Symbol.RFID3;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Symbol.RFID3.Events;
using static Symbol.RFID3.TagAccess;

namespace RFIDSolution.TestReader
{
    class Program
    {
        static RFIDReader m_ReaderAPI;
        static TagData[] tagData;

        static void Main(string[] args)
        {
            string ip = "192.168.0.111";
            uint port = 5084;
            m_ReaderAPI = new RFIDReader(ip, port, 0);
            m_ReaderAPI.Connect();
            m_ReaderAPI.Actions.PurgeTags();
            Antennas.Config antennaConfig = m_ReaderAPI.Config.Antennas[1].GetConfig();
            antennaConfig.TransmitPowerIndex = 132;
            m_ReaderAPI.Config.Antennas[1].SetConfig(antennaConfig);

            //readTag();
            mInventory();
            Console.ReadLine();
        }

        private static void readTag()
        {
            while (true)
            {
                Thread.Sleep(100);
                ReadAccessParams accessParams = new ReadAccessParams();
                m_ReaderAPI.Actions.TagAccess.ReadEvent(accessParams, null, null);
                TagData[] tagData = m_ReaderAPI.Actions.GetReadTags(1000);
                Console.WriteLine("Reading...");
                if (tagData == null) continue;

                for (int tagIndex = 0; tagIndex < tagData.Length; tagIndex++)
                {
                    TagData tag = tagData[tagIndex];
                    showData(tag);
                }
            }
        }

        private static async void mInventory()
        {
            PostFilter postFilter = null;

            TriggerInfo triggerInfo = new TriggerInfo();
            triggerInfo.StartTrigger.Type = START_TRIGGER_TYPE.START_TRIGGER_TYPE_IMMEDIATE;
            triggerInfo.StopTrigger.Type = STOP_TRIGGER_TYPE.STOP_TRIGGER_TYPE_IMMEDIATE;
            if (m_ReaderAPI.ReaderCapabilities.IsTagEventReportingSupported)
            {
                triggerInfo.TagEventReportInfo.ReportNewTagEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                triggerInfo.TagEventReportInfo.ReportTagBackToVisibilityEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                triggerInfo.TagEventReportInfo.ReportTagInvisibleEvent = TAG_EVENT_REPORT_TRIGGER.MODERATED;
                triggerInfo.TagEventReportInfo.NewTagEventModeratedTimeoutMilliseconds = 500;
                triggerInfo.TagEventReportInfo.TagBackToVisibilityModeratedTimeoutMilliseconds = 500;
                triggerInfo.TagEventReportInfo.TagInvisibleEventModeratedTimeoutMilliseconds = 500;
            }
            triggerInfo.TagReportTrigger = 1;
            triggerInfo.ReportTriggers.Period = 0;

            m_ReaderAPI.Events.ReadNotify += new ReadNotifyHandler(myUpdateRead);

            AntennaInfo antennaInfo = null;

            Console.WriteLine("EPC | AntenId | Last seen");
            m_ReaderAPI.Actions.Inventory.Perform(
                postFilter,
                triggerInfo,
                antennaInfo);
            //while (true)
            //{
            //    await Task.Delay(100);
            //    tagData = m_ReaderAPI.Actions.GetReadTags(1000);
            //    //Console.WriteLine("Event tag" + e.ReadEventData?.TagData?.TagID);
            //    Console.WriteLine("Triggering reader...");
            //    if (tagData == null) continue;

            //    for (int tagIndex = 0; tagIndex < tagData.Length; tagIndex++)
            //    {
            //        TagData tag = tagData[tagIndex];
            //        showData(tag);
            //    }
            //}
        }

        private static void myUpdateRead(object sender, ReadEventArgs e)
        {
            tagData = m_ReaderAPI.Actions.GetReadTags(1000);
            //TagData[] tagData = m_ReaderAPI.Actions.GetReadTags(1000);
            //Console.WriteLine("Event tag" + e.ReadEventData?.TagData?.TagID);
            if (tagData == null) return;
            Console.WriteLine("Triggering reader... " + tagData.Length);

            for (int tagIndex = 0; tagIndex < tagData.Length; tagIndex++)
            {
                TagData tag = tagData[tagIndex];
                showData(tag);
            }
        }

        private static void showData(TagData tag)
        {
            Console.WriteLine($"{tag.TagID} | {tag.AntennaID} | {tag.CRC}");
        }

    }
}
