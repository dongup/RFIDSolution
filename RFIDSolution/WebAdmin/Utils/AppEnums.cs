using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSolution.WebAdmin.Utils
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
            Available = 1,
            NotAvailable = 2,
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
            Center,
            Left,
            Right
        }
    }
}
