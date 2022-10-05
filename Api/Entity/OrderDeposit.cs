using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class OrderDeposit
    {
        public long ID { get; set; }
        public string OrderNo { get; set; }
        public string OpenID { get; set; }
        public decimal PayTotal { get; set; }
        public decimal ApplyTotal { get; set; }
        public string ApplyTime { get; set; }
        public decimal RefundTotal { get; set; }

        // 状态  100-已支付  200-退还中  202-退还失败 300-已退还
        public int Status { get; set; }
        public string CreateTime { get; set; }
        public string StatusDesc
        {
            get
            {
                return OrderStatus.DepositStatus_Zh(Status);
            }

        }

        public decimal DeductionTotal { get; internal set; }
        public string DeductionReason { get; internal set; }
        /// <summary>
        /// 滞纳金
        /// </summary>
        public decimal PenaltyTotal { get; set; }
        public string RefundTime { get; set; }
    }
}
