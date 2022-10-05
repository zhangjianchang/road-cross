using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class PartnerFormParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string KeyWords { get; set; }
        public int? Status { get; set; }
    }
    public class PartnerApproveData
    {
        public string ID { get; set; }
        public string DataType { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Organization { get; set; }
        public string UpdateBy { get; set; }
    }

    public class PartnerForm
    {

        public string ID { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string OpenID { get; set; }
        /// <summary>
        /// 状态（0-待审核；100-审核通过；200-已拒绝）
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 支付状态（100-未支付；200-已支付；300-已取消；）
        /// </summary>
        public int PayStatus { get; set; }
        public string BindPartnerUserID { get; set; }
        public string CreateTime { get; set; }
        public string CreateBy { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        /// <summary>
        /// 瓶子状态描述
        /// </summary>
        public string StatusDesc
        {
            get
            {
                return Status switch
                {
                    0 => "待审核",
                    100 => "审核通过",
                    200 => "已拒绝",
                    _ => "--",
                };
            }
        }

        public string PayStatusDesc
        {
            get
            {
                return PayStatus switch
                {
                    100 => "未支付",
                    200 => "已支付",
                    300 => "已取消",
                    _ => "--",
                };
            }
        }

    }

    public class PartnerFormGuide : PartnerForm
    {
        public string IDCard { get; set; }
        public string Attachment { get; set; }
    }

    public class PartnerFormGeneral : PartnerForm
    {
        public string MerchantType { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAddress { get; set; }
        public string MerchantScale { get; set; }
        public string BusinessType { get; set; }
        public string IsChain { get; set; }
    }
    public class PartnerFormAdvanced : PartnerForm
    {
        public string MerchantType { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAddress { get; set; }
        public string MerchantScale { get; set; }
        public string BusinessType { get; set; }
        public string IsChain { get; set; }
    }

}
