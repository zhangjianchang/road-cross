using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class CouponsCustomerParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OpenID { get; set; }
        public long? CouponID { get; set; }
        public string Telphone { get; set; }
        public string MemberType { get; set; }
        public int? IsSend { get; set; }
        public string RegistTimeStart { get; set; }
        public string RegistTimeEnd { get; set; }
        public string CreateBy { get; set; }
    }

    public class CouponsCustomer
    {
        public long ID { get; set; }
        public string OpenID { get; set; }
        public long CouponID { get; set; }
        public string CouponName { get; set; }
        public string Telphone { get; set; }
        public string RegistTime { get; set; }
        public string MemberType { get; set; }
        public int Status { get; set; }
        public string GiveTime { get; set; }
        public string UseTime { get; set; }
        public string StatusDesc
        {
            get
            {
                return Status switch
                {
                    100 => "未使用",
                    200 => "已使用",
                    _ => "--",
                };
            }
        }
        public string UpdateBy { get; set; }

        public string MemberTypeDesc
        {
            get { return GetMemberTypeDesc(MemberType); }
        }

        /// <summary>
        /// 获取会员级别展示文本
        /// </summary>
        public static string GetMemberTypeDesc(string type)
        {
            return type switch
            {
                "ordinary" => "普通会员",
                "silver" => "银卡会员",
                "gold" => "金卡会员",
                _ => "--",
            };
        }

        public int IsSend { get; set; }

    }
}
