using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Models
{
    public class TableState
    {
        public TableState()
        {

        }

        public TableState(int pageItem)
        {
            PageItem = pageItem;
        }

        public int PageItem { get; set; } = 10;

        public int PageIndex { get; set; } = 0;

        public int TotalPage
        {
            get
            {
                int total = 0;
                total = (int)(TotalRow / PageItem);
                if (PageIndex > total)
                {
                    PageIndex = total;
                } 
                else if (PageIndex < 0)
                {
                    PageIndex = 0;
                }
                return total;
            }
        }

        public int TotalRow { get; set; } = 70;

        public int FirstIndex
        {
            get
            {
                int index = PageItem * PageIndex + 1;
                return index;
            }
        }

        public int LastIndex
        {
            get
            {
                int index = FirstIndex + PageItem - 1;
                if(index > TotalRow)
                {
                    index = TotalRow;
                }
                return index;
            }
        }
    }
}
