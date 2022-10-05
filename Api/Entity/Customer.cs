using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entity
{
    public class CustomerParam
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Telphone { get; set; }
        public string MemberType { get; set; }
        public int? Status { get; set; }
    }

    public class Customer
    {
        public string OpenID { get; set; }
        public string Telphone { get; set; }
        public string RegistTime { get; set; }
        public string MemberType { get; set; }
        public decimal MemberPoints { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// 升级记录数
        /// </summary>
        public int LevelupRecordCount { get; set; }
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

        //同步老系统数据
        public string WXPayScoreState { get; set; }
        public string WXAuthorizationCode { get; set; }
        public string WXPayScoreOpenId { get; set; }
        public string IsSend { get; set; }

    }

    public class LevelupRecord
    {
        public string OrderNo { get; set; }
        public string TargetLevel { get; set; }
        public string TargetLevelDesc
        {
            get { return Customer.GetMemberTypeDesc(TargetLevel); }
        }
        public string CurrentLevel { get; set; }
        public string CurrentLevelDesc
        {
            get { return Customer.GetMemberTypeDesc(CurrentLevel); }
        }
        public string PayWay { get; set; }
        public decimal TotalAmount { get; set; }
        public string PayStatus { get; set; }
        public string CreateTime { get; set; }
        public string PayWayDesc
        {
            get
            {
                return PayWay switch
                {
                    "wechat" => "微信支付",
                    "points" => "积分升级",
                    _ => "--",
                };
            }
        }
    }

    public class PointsRecord
    {
        public string Telphone { get; set; }
        public string Points { get; set; }
        public string Remark { get; set; }
        public string ScoreWay { get; set; }
        public string OrderNo { get; set; }
        public string CreateTime { get; set; }
    }
}
