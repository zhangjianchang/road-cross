namespace Api.Entity
{
    public class AuthotizationCode
    {
        public string ID { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// 类型：1月卡2年卡3自定义时长4企业会员
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 当Type=3时，自定义时长的值
        /// </summary>
        public decimal Duration { get; set; }
        public string TypeName => GetNameByType(Type);
        public string MemberName { get; set; }
        public decimal ValidDate { get; set; }
        public string ActiveDate { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public string ExpireDate { get; set; }
        /// <summary>
        /// 使用次数
        /// </summary>
        public decimal UsageTimes { get; set; }
        /// <summary>
        /// 已使用次数
        /// </summary>
        public decimal UsedTimes { get; set; }
        /// <summary>
        /// 剩余次数
        /// </summary>
        public decimal RemainingTimes { get; set; }
        /// <summary>
        /// 剩余天数
        /// </summary>
        public decimal RemainingDays { get; set; }
        public string CreateDate { get; set; }
        /// <summary>
        /// 状态，100待激活；200已激活；300次数使用完毕；400已过期
        /// </summary>
        public string Status { get; set; }
        public string StatusName => GetNameByCode(Status);

        public static string GetNameByType(string type)
        {
            return type switch
            {
                "1" => "月卡",
                "2" => "年卡",
                "3" => "自定义月卡",
                "4" => "企业年卡",
                _ => "异常",
            };
        }

        public static string GetNameByCode(string code)
        {
            return code switch
            {
                "100" => "未激活",
                "200" => "已激活",
                "300" => "次数使用完毕",
                "400" => "已过期",
                _ => "状态异常",
            };
        }
    }

    public class AuthotizationCodeRequest : AuthotizationCode { }
}
