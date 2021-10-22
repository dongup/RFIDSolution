using RFIDSolution.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Models
{
    public class PaginationResponse<T>
    {
        public PaginationResponse()
        {

        }

        public PaginationResponse(IQueryable<T> lst, int pageItem, int pageIndex, bool isPagging = true)
        {
            this.pageIndex = pageIndex;
            this.pageItem = pageItem;

            TotalRow = lst.Count();

            if (pageItem == 1)
            {
                TotalPage = TotalRow;
            }
            else if (TotalRow == pageItem)
            {
                TotalPage = 1;
            }
            else
            {
                if (TotalRow % pageItem == 0)
                {
                    TotalPage = TotalRow / pageItem;
                }
                else
                {
                    TotalPage = (TotalRow / pageItem) + 1;
                }
            }

            if(TotalRow < pageItem)
            {
                pageIndex = 0;
            }

            if (isPagging)
            {
                Data = lst.Pagging(pageItem, pageIndex).ToList();
            }
        }

        private int pageIndex { get; set; }

        private int pageItem { get; set; }

        public int TotalPage { get; set; }

        public int TotalRow { get; set; }

        public List<T> Data { get; set; }
    }
}
