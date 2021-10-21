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
        public enum GPOPortType
        {
            [Description("Light power")]
            Power = 0,
            [Description("Green light")]
            Green = 1,
            [Description("Red light")]
            Red = 2,
        }

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
            //Đã đem ra ngoài
            [Description("Transfered out")]
            Transfered = 2,
            //Đã đem xuất ra nước ngoài
            [Description("Deliveried out")]
            DeliveryOut = 3,
            //Không được phép mượn
            [Description("Unavailable")]
            Unavailable = 4,
            //Tag chưa được mapping
            [Description("Unknown")]
            Unknown = 5
        }

        /// <summary>
        /// Trạng thái mượn trả sản phẩm
        /// </summary>
        public enum InoutStatus
        {
            [Description("Borrowing")]
            Borrowing = 1,
            [Description("Returned")]
            Returned = 2,
            [Description("Delivered")]
            Delivered = 3,
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
            //Chờ kiểm
            Pending = 1,
            //Đang kiểm
            OnGoing = 2,
            //Đã kiểm xong
            Completed = 3,
            //Đã hủy
            Canceled = 4
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

        public enum AntennaLocation
        {
            Table = 1,
            CheckPoint = 2
        }

        public enum AntennaStatus
        {
            [Description("Connected")]
            Connected = 1,
            [Description("Disconnected")]
            Disconnected = 2,
            [Description("Unknown")]
            Unknown = 3
        }

        public enum GPIPortStatus
        {
            High = 1,
            Low = 2,
            Unknown = 3
        }  
        
        public enum RdrLog
        {
            [Description("Connect")]
            Connect = 1,
            [Description("Disconnect")]
            Disconnect = 2,
            [Description("Error")]
            Error = 3,
            [Description("Info")]
            Info = 4
        }

        public enum UserStatus
        {
            [Description("Active")]
            Active = 1,
            [Description("Disabled")]
            Disabled = 2
        }

        public enum LogLevelEnum
        {
            [Description("Log lỗi")]
            Error = 1,
            [Description("Log update")]
            Put = 2,
            [Description("Log delete")]
            Delete = 3,
            [Description("Log thông tin")]
            Info = 4
        }

        public enum BadgeColor
        {
            Error,
            Warning,
            Success,
            Info,
            Primary
        }

        public enum TransferType { 
            [Description("Internal")]
            Internal = 1,
            [Description("Delivery")]
            Delivery = 2
        }
    }
}
