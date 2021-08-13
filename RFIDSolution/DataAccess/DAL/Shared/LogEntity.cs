﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RFIDSolution.WebAdmin.DAL.Entities
{
    [Table("Logs")]
    public class LogEntity : BaseEntity
    {
        public LogEntity() : base()
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

        [StringLength(200)]
        public string UserAgent { get; set; }

        [StringLength(20)]
        public string OperationSystemVersion { get; set; }

        public string Token { get; set; }

        [StringLength(50)]
        public string RequestIpAddress { get; set; }

        /// <summary>
        /// 1: Information; 2: Error; 3: Warring
        /// </summary>
        public LogLevel Level { get; set; }

        [NotMapped]
        public bool showDetail = false;

        [NotMapped]
        public string ColorClass => getColorClass();

        private string getColorClass()
        {
            string color = "badge ";
            switch (Level)
            {
                case LogLevel.Error:
                    color += "badge-danger";
                    break;
                case LogLevel.Put:
                    color += "badge-success";
                    break;
                case LogLevel.Delete:
                    color += "badge-warning";
                    break;
                case LogLevel.Info:
                    color += "badge-info";
                    break;
                default:
                    break;
            }
            return color;
        }

        public enum LogLevel
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

        public int? RequestUserId { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionDetail { get; set; }
    }
}
