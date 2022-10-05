using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public enum CabinetStatusEnum
    {
        /// <summary>
        /// 可用
        /// </summary>
        Available = 1,
        /// <summary>
        /// 不可用
        /// </summary>
        Disabled = 2,
        /// <summary>
        /// 维修
        /// </summary>
        Repairing = 3,
    }
    public static class CabinetStatus
    {
        public static string StatusDesc(string status)
        {
            switch (status)
            {
                case "1": return "可用";
                case "2": return "不可用";
                case "3": return "维修";
                default: return "--";
            }
        }

        public static List<FilterOptions> GetStatusOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            options.Add(new FilterOptions { Label = "可用", Value = "1" });
            options.Add(new FilterOptions { Label = "不可用", Value = "2" });
            options.Add(new FilterOptions { Label = "维修", Value = "3" });
            return options;
        }
    }



    public enum CabinetDeviceStatusEnum
    {
        /// <summary>
        /// 在线
        /// </summary>
        Online = 1,
        /// <summary>
        /// 离线
        /// </summary>
        Offine = 0,
    }
    public static class CabinetDeviceStatus
    {
        public static string DeviceStatusDesc(string status)
        {
            switch (status)
            {
                case "1": return "在线";
                case "0": return "离线";
                default: return "--";
            }
        }

        public static List<FilterOptions> GetDeviceStatusOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            options.Add(new FilterOptions { Label = "在线", Value = "1" });
            options.Add(new FilterOptions { Label = "离线", Value = "0" });
            return options;
        }
    }

}
