using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Models
{
    public class DateRange
    {
        public DateRange()
        {

        }

        public DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateRange(int month)
        {
            var now = DateTime.Now;
            Start = new DateTime(now.Year, month, 1);
            End = new DateTime(now.Year, month, DateTime.DaysInMonth(now.Year, month));
        }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
