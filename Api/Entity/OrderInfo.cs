using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class OrderInfo
    {
        public string OrderNo { get; set; }
        public string CabinetId { get; set; }
        public string Address { get; set; }
        public string CabinetName { get; set; }
        public string OpenId { get; set; }
        public decimal OriginalPrice { get; set; }
        public int Count { get; set; }
        public decimal Deposit { get; set; }
        public int CouponsId { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public int PayStatus { get; set; }
        public string PayStatusDesc { get; set; }
        public int Status { get; set; }
        public string StatusDesc { get; set; }
        public int PayWay { get; set; }
        public string Desc { get; set; }
        public string Telphone { get; set; }
        public string OverdueTime { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public int DepositStatus { get; set; }
        public string PayWayDesc
        {
            get
            {
                return PayWay switch
                {
                    1 => "微信支付",
                    2 => "微信信用分",
                    _ => "--",
                };
            }
        }
        public int CanRefund { get; set; }
        /// <summary>
        /// 逾期瓶子数
        /// </summary>
        public int OverdueCount { get; set; }
        /// <summary>
        /// 会员类型
        /// </summary>
        public string MemberType { get; set; }
    }

    public class OrderInfoParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OrderNo { get; set; }
        public string Telphone { get; set; }
        public string Status { get; set; }
        public int? IsOverdue { get; set; }
        public string CreateTimeStartStr { get; set; }
        public string CreateTimeEndStr { get; set; }
        /// <summary>
        /// 订单支付金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 订单退款金额
        /// </summary>
        public decimal RefundTotal { get; set; }
        /// <summary>
        /// 滞纳金
        /// </summary>
        public decimal PenaltyTotal { get; set; }
        /// <summary>
        /// 其它扣款
        /// </summary>
        public decimal DeductionTotal { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 会员类型
        /// </summary>
        public string MemberType { get; set; }
    }
}
