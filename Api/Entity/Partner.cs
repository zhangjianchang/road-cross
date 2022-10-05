using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class PartnerParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string KeyWords { get; set; }
        public string Type { get; set; }
        public string Organization { get; set; }
        public int? IsOwner { get; set; }
        public int? Status { get; set; }
    }

    public class Partner
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public int IsOwner { get; set; }
        public string OwnerID { get; set; }
        /// <summary>
        /// general-普通；advanced-高级；touristguide-导游
        /// </summary>
        public string Type { get; set; }
        public string TypeDesc
        {
            get
            {
                switch (Type)
                {
                    case "general": return "普通合作伙伴";
                    case "advanced": return "高级合作伙伴";
                    case "touristguide": return "导游";
                    default: return "";
                }
            }
        }
        /// <summary>
        /// 账号状态（1-启用；0-禁用）
        /// 提现状态（100-申请/冻结；200-已打款）
        /// </summary>
        public int Status { get; set; }
        public string StatusDesc
        {
            get
            {
                switch (Status)
                {
                    case 1: return "有效";
                    case 0: return "无效";
                    case 100: return "未打款";
                    case 200: return "已打款";
                    default: return "";
                }
            }
        }

        /// <summary>
        /// 绑定箱子数
        /// </summary>
        public int BindBoxCount { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        /// <summary>
        /// 提现金额
        /// </summary>
        public string Amount { get; set; }

    }

}
