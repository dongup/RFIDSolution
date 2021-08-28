using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.Shared.Enums
{
    public class AppEnums
    {
        /// <summary>
        /// Giầy trái hay phải
        /// </summary>
        public enum ProductSide
        {
            Left = 1,
            Right = 2
        }

        public enum ProductStatus
        {
            //Đang trong kho
            [Description("Available")]
            Available = 1,
            //Đã cho mượn
            [Description("Not Available")]
            NotAvailable = 2,
            //Không được phép mượn
            [Description("On Hold")]
            OnHold = 3
        }

        /// <summary>
        /// Trạng thái mượn trả sản phẩm
        /// </summary>
        public enum InoutStatus
        {
            Borrowing = 1,
            Returned = 2
        }

        /// <summary>
        /// Trạng thái sản phẩm khi lấy/trả
        /// </summary>
        public enum GetStatus
        {
            Ok = 1,
            NotOk = 2
        }

        public enum AlterConfirmStatus
        {
            Pending = 1,
            Confirmed = 2
        }

        public enum InventoryStatus
        {
            Pending = 1,
            Completed = 2
        }

        public enum InventoryProductStatus
        {
            Found = 1,
            NotFound = 2
        }

        public enum RFIDTagStatus
        {
            Available = 1,
            Discarded = 2
        }

        public enum Alignment
        {
            Left,
            Right,
            Center
        }

        public enum AntennaLoction
        {
            Table = 1,
            CheckPoint = 2
        }
    }
}
