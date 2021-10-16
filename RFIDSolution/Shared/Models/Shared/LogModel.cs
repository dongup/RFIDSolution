using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.Shared.DAL.Entities
{
    public class LogModel
    {
        public LogModel() : base()
        {

        }

        [Key]
        public int Id { get; set; }

        [StringLength(10)]
        public string Method { get; set; }

        public string RequestUrl { get; set; }

        /// <summary>
        /// Nội dung log
        /// </summary>
        public string LogContent { get; set; }

        /// <summary>
        /// Dữ liệu gửi lên
        /// </summary>
        public string RequestBody { get; set; }

        public string UserAgent { get; set; }

        public string OperationSystemVersion { get; set; }

        public string Token { get; set; }

        public string RequestIpAddress { get; set; }

        /// <summary>
        /// 1: Information; 2: Error; 3: Warring
        /// </summary>
        public LogLevelEnum Level { get; set; }

        public bool ShowDetail = false;

        public string ColorClass => getColorClass();

        private string getColorClass()
        {
            string color = "badge ";
            switch (Level)
            {
                case LogLevelEnum.Error:
                    color += "badge-danger";
                    break;
                case LogLevelEnum.Put:
                    color += "badge-success";
                    break;
                case LogLevelEnum.Delete:
                    color += "badge-warning";
                    break;
                case LogLevelEnum.Info:
                    color += "badge-info";
                    break;
                default:
                    break;
            }
            return color;
        }
       
        public DateTime CreatedDate { get; set; }

        public int? RequestUserId { get; set; }

        public string RequestUser { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionDetail { get; set; }
    }
}
