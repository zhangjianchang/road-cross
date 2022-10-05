using Api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class CouponsParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int? RuleType { get; set; }
        public string KeyWords { get; set; }
        public int? Status { get; set; }
    }
    public class Coupons
    {
        public long ID { get; set; }
        /// <summary>
        /// 券名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 券的标题（如：9折）
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 券的副标题（如：无门槛礼券）
        /// </summary>
        public string SubTitle { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TimeSpan
        {
            get
            {
                return $"{StartTime}至{EndTime}";
            }
        }
        /// <summary>
        /// 是否用户注册自动发放
        /// </summary>
        public int IsAutoGive { get; set; }
        /// <summary>
        /// 规则类型： 1-满减；2-打折
        /// </summary>
        public int RuleType { get; set; }
        public string RuleTypeDesc
        {
            get
            {
                switch (RuleType)
                {
                    case 1: return "满减";
                    case 2: return "折扣";
                    default: return "--";
                }
            }
        }
        /// <summary>
        /// 门槛值
        /// </summary>
        public decimal Threshold { get; set; }
        /// <summary>
        /// 优惠值（当规则类型为打折时，0.95代表9.5折）
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 优惠描述
        /// </summary>
        public string Desc
        {
            get
            {
                switch (RuleType)
                {
                    case 1: return (Threshold > 0 ? $"满 {Threshold.ToString("0.##")} 元 " : "无门槛 立") + $"减 {Discount.ToString("0.##")} 元";
                    case 2: return (Threshold > 0 ? $"满 {Threshold.ToString("0.##")} 元 " : "无门槛 ") + $"打 {(Discount).ToString("0.##")} 折";
                    default: return "--";
                }
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public string StatusDesc
        {
            get
            {
                if (DateTime.Now >= Converter.TryToDateTime(StartTime) && DateTime.Now <= Converter.TryToDateTime(EndTime))
                {
                    return "可用";
                }
                else
                {
                    return "不可用";
                }
            }
        }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public bool IsNew { get; set; }
    }
}
