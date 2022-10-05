using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public enum GridStatusEnum
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
        /// <summary>
        /// 在线
        /// </summary>
        Online = 4,
        /// <summary>
        /// 离线
        /// </summary>
        Offine = 5,
    }
    public static class GridStatus
    {
        public static string StatusDesc(string status)
        {
            switch (status)
            {
                case "1": return "可用";
                case "2": return "不可用";
                case "3": return "维修";
                case "4": return "在线";
                case "5": return "离线";
                default: return "--";
            }
        }

        public static List<FilterOptions> GetStatusOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            options.Add(new FilterOptions { Label = "可用", Value = "1" });
            options.Add(new FilterOptions { Label = "不可用", Value = "2" });
            options.Add(new FilterOptions { Label = "维修", Value = "3" });
            options.Add(new FilterOptions { Label = "在线", Value = "4" });
            options.Add(new FilterOptions { Label = "离线", Value = "5" });
            return options;
        }
    }
}
