﻿using RFIDSolution.Shared.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RFIDSolution.Shared.Enums.AppEnums;

namespace RFIDSolution.DataAccess.DAL.Entities
{
    public class AntennaEntity : BaseEntity
    {
        public AntennaEntity() : base()
        {

        }

        /// <summary>
        /// ID của hệ thống
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Id của anten trên đầu đọc
        /// </summary>
        public int ANTENNA_ID { get; set; }

        /// <summary>
        /// Tên anten do người dùng đặt
        /// </summary>
        public string ANTENNA_NAME { get; set; }

        /// <summary>
        /// Vị trí đặt anten do người dùng config
        /// </summary>
        public AntennaLocation LOCATION { get; set; } = AntennaLocation.Table;

        /// <summary>
        /// Cường độ của anten
        /// </summary>
        public int ANTENNA_POWER { get; set; }
    }
}
