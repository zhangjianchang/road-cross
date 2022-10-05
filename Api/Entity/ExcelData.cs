namespace Api.Entity
{
    public class ExcelData
    {
        public static string GetMemberType_Zh(string member)
        {
            return member switch
            {
                "gold" => "金卡会员",
                "silver" => "银卡会员",
                _ => "普通会员",
            };
        }
    }

    [EnitityMapping(TableName = "平台业务基础数据统计表")]
    public class PlatformBusinessData
    {
        [EnitityMapping(ColumnName = "统计时间")]
        public string CreateTime { get; set; }

        [EnitityMapping(ColumnName = "会员")]
        public string MemberType_Zh => ExcelData.GetMemberType_Zh(MemberType);


        public string MemberType { get; set; }

        [EnitityMapping(ColumnName = "增量")]
        public decimal IncreaseCount { get; set; }

        [EnitityMapping(ColumnName = "租赁情况")]
        public decimal LeasingSituation { get; set; }

        [EnitityMapping(ColumnName = "未还瓶数")]
        public decimal UnreturnedBottles { get; set; }

        [EnitityMapping(ColumnName = "租赁价格")]
        public decimal OriginalPrice { get; set; }

        [EnitityMapping(ColumnName = "押金汇总")]
        public decimal Deposit { get; set; }

        [EnitityMapping(ColumnName = "滞纳金")]
        public decimal Penalty { get; set; }

        [EnitityMapping(ColumnName = "市场公司分润")]
        public decimal MarketProfit { get; set; }

        [EnitityMapping(ColumnName = "平台公司分润")]
        public decimal PlatformProfit { get; set; }

        [EnitityMapping(ColumnName = "宣传公司分润")]
        public decimal PublicityProfit { get; set; }

        [EnitityMapping(ColumnName = "运营公司分润")]
        public decimal OperationProfit { get; set; }

        [EnitityMapping(ColumnName = "商家返利")]
        public decimal MerchantRebate { get; set; }
    }


    [EnitityMapping(TableName = "会员情况统计")]
    public class MemberData
    {
        [EnitityMapping(ColumnName = "会员类型")]
        public string MemberType_Zh => ExcelData.GetMemberType_Zh(MemberType);

        public string MemberType { get; set; }

        [EnitityMapping(ColumnName = "数量")]
        public decimal Count { get; set; }
    }

    [EnitityMapping(TableName = "商家返利统计")]
    public class ProfitData
    {
        [EnitityMapping(ColumnName = "商户名称")]
        public string CabinetName { get; set; }

        [EnitityMapping(ColumnName = "配发瓶次累计")]
        public decimal AllotmentCount { get; set; }

        [EnitityMapping(ColumnName = "配发金额累计")]
        public decimal AllotmentAmount { get; set; }

        [EnitityMapping(ColumnName = "回收瓶次累计")]
        public decimal RecycleCount { get; set; }

        [EnitityMapping(ColumnName = "回收金额累计")]
        public decimal RecycleAmount { get; set; }

        [EnitityMapping(ColumnName = "销售提成-金卡瓶次")]
        public decimal GoldCount { get; set; }

        [EnitityMapping(ColumnName = "销售提成-银卡瓶次")]
        public decimal SilverCount { get; set; }

        [EnitityMapping(ColumnName = "销售提成-普通瓶次")]
        public decimal OrdinaryCount { get; set; }

        [EnitityMapping(ColumnName = "销售提成-金额累计")]
        public decimal SalesAmount { get; set; }

        [EnitityMapping(ColumnName = "总金额合计")]
        public decimal TotalAmount => AllotmentAmount + RecycleAmount + SalesAmount;
    }

    public class ProfitDataRequest
    {
      public string CreateTimeS { get; set; }
      public string CreateTimeE { get; set; }
    }
}
