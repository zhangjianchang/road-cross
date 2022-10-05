using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public enum BottleStatusEnum
    {
        /// <summary>
        /// 未取瓶
        /// </summary>
        ToBeTaken = 1,
        /// <summary>
        /// 租赁中
        /// </summary>
        Processing = 2,
        /// <summary>
        /// 已归还
        /// </summary>
        Finished = 3,
    }
    public static class BottleStatus
    {
        public static string StatusDesc(string status)
        {
            return status switch
            {
                "1" => "未取瓶",
                "2" => "租赁中",
                "3" => "已归还",
                _ => "--",
            };
        }

        public static List<FilterOptions> GetStatusOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>
            {
                new FilterOptions { Label = "未取瓶", Value = "1" },
                new FilterOptions { Label = "租赁中", Value = "2" },
                new FilterOptions { Label = "已归还", Value = "3" }
            };
            return options;
        }
    }
}
