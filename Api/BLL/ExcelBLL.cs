using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Api.BLL
{
    public class ExcelBLL
    {
        /// <summary>
        /// 查询平台基础数据
        /// </summary>
        /// <param name="type">data：十五天数据，summary：统计数据</param>
        /// <returns></returns>
        public static List<PlatformBusinessData> GetPlatformBusinessData(string type = "data")
        {
            List<PlatformBusinessData> list = new List<PlatformBusinessData>();
            string procName = type == "data" ? "Proc_Platform_Business_Data" : "Proc_Platform_Business_Summary";
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, CommandType.StoredProcedure, procName);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new PlatformBusinessData()
                    {
                        CreateTime = Converter.TryToString(row["CreateTime"]),
                        MemberType = Converter.TryToString(row["MemberType"]),
                        IncreaseCount = Converter.TryToDecimal(row["IncreaseCount"]),
                        LeasingSituation = Converter.TryToDecimal(row["LeasingSituation"]),
                        UnreturnedBottles = Converter.TryToDecimal(row["UnreturnedBottles"]),
                        OriginalPrice = Converter.TryToDecimal(row["OriginalPrice"]),
                        Deposit = Converter.TryToDecimal(row["Deposit"]),
                        Penalty = Converter.TryToDecimal(row["Penalty"]),
                        MarketProfit = Converter.TryToDecimal(row["MarketProfit"]),
                        PlatformProfit = Converter.TryToDecimal(row["PlatformProfit"]),
                        PublicityProfit = Converter.TryToDecimal(row["PublicityProfit"]),
                        OperationProfit = Converter.TryToDecimal(row["OperationProfit"]),
                        MerchantRebate = Converter.TryToDecimal(row["MerchantRebate"]),
                    });
                }
            }
            return list;
        }

        /// <summary>
        /// 导出平台基础数据
        /// </summary>
        /// <param name="type">data：十五天数据，summary：统计数据</param>
        /// <returns></returns>
        public static byte[] ExportPlatformBusinessData(string type = "data")
        {
            List<PlatformBusinessData> list = GetPlatformBusinessData(type);
            //表名
            //tableName = ExcelHelper.GetTableName<PlatformBusinessData>();
            //表头
            Dictionary<string, string> dic = ExcelHelper.GetSheetHeader<PlatformBusinessData>();
            //表内容转换
            ExcelHelper excelHelper = new ExcelHelper(dic);
            var result = excelHelper.Export(list, true);
            return result;
        }


        /// <summary>
        /// 查询会员总数
        /// </summary>
        /// <returns></returns>
        public static List<MemberData> GetMemberData()
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, CommandType.StoredProcedure, "Proc_Member_Data");
            if (dt != null && dt.Rows.Count > 0)
            {
                var list = new List<MemberData>();
                foreach (DataRow row in dt.Rows)
                {
                    var data = new MemberData()
                    {
                        MemberType = Converter.TryToString(row["MemberType"]),
                        Count = Converter.TryToDecimal(row["Count"]),
                    };
                    list.Add(data);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 导出会员总数
        /// </summary>
        /// <returns></returns>
        public static byte[] ExportMemberData()
        {
            List<MemberData> list = GetMemberData();
            //表名
            //tableName = ExcelHelper.GetTableName<MemberData>();
            //表头
            Dictionary<string, string> dic = ExcelHelper.GetSheetHeader<MemberData>();
            //表内容转换
            ExcelHelper excelHelper = new ExcelHelper(dic);
            var result = excelHelper.Export(list);
            return result;
        }

        /// <summary>
        /// 查询商家返利统计
        /// </summary>
        /// <returns></returns>
        public static List<ProfitData> GetProfitData(ProfitDataRequest request)
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, CommandType.StoredProcedure, "Proc_Profit_Data", 
                new MySqlParameter("@CreateTimeS", request.CreateTimeS), 
                new MySqlParameter("@CreateTimeE", request.CreateTimeE));   
            if (dt != null && dt.Rows.Count > 0)
            {
                var list = new List<ProfitData>();
                foreach (DataRow row in dt.Rows)
                {
                    var data = new ProfitData()
                    {
                        CabinetName = Converter.TryToString(row["CabinetName"]),
                        AllotmentCount = Converter.TryToDecimal(row["AllotmentCount"]),
                        AllotmentAmount = Converter.TryToDecimal(row["AllotmentAmount"]),
                        RecycleCount = Converter.TryToDecimal(row["RecycleCount"]),
                        RecycleAmount = Converter.TryToDecimal(row["RecycleAmount"]),
                        GoldCount = Converter.TryToDecimal(row["GoldCount"]),
                        SilverCount = Converter.TryToDecimal(row["SilverCount"]),
                        OrdinaryCount = Converter.TryToDecimal(row["OrdinaryCount"]),
                        SalesAmount = Converter.TryToDecimal(row["SalesAmount"]),
                    };
                    list.Add(data);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// 导出商家返利统计
        /// </summary>
        /// <returns></returns>
        public static byte[] ExportProfitData(ProfitDataRequest request)
        {
            List<ProfitData> list = GetProfitData(request);
            //表名
            //tableName = ExcelHelper.GetTableName<ProfitData>();
            //表头
            Dictionary<string, string> dic = ExcelHelper.GetSheetHeader<ProfitData>();
            //表内容转换
            ExcelHelper excelHelper = new ExcelHelper(dic);
            var result = excelHelper.Export(list);
            return result;
        }
    }
}
