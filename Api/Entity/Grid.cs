using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class Grid
    {
        public bool IsAdd { get; set; }
        public string ID { get; set; }
        public string CabinetID { get; set; }
        public string CabinetNumber { get; set; }
        public string CabinetName { get; set; }
        public string GridDeviceCode { get; set; }
        public string GridNumber { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public string DoorStatus { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public string GridID { get; set; }
        public string BottleID { get; set; }
        public string BottleCode { get; set; }
        public string BottleSpec { get; set; }

        /// <summary>
        /// 格子状态描述
        /// </summary>
        public string StatusDesc
        {
            get
            {
                return GridStatus.StatusDesc(Status);
            }
        }
        /// <summary>
        /// 格子状态描述
        /// </summary>
        public string DoorStatusDesc
        {
            get
            {
                switch (DoorStatus)
                {
                    case "1": return "开门";
                    case "0": return "关门";
                    default: return "--";
                }
            }
        }
        /// <summary>
        /// 格子状态描述
        /// </summary>
        public string BottleDesc
        {
            get
            {
                return string.IsNullOrEmpty(BottleCode) ? "" : BottleCode + "/" + BottleSpec;
            }
        }
    }


    public class GridParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string CabinetID { get; set; }
        public string GridNumber { get; set; }
        public string Status { get; set; }
    }
}
