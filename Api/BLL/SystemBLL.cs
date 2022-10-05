using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Api.BLL
{
    public class SystemBLL
    {
        internal static List<ApiLog> GetApiLogList(ApiLogParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<ApiLog> recordList = new List<ApiLog>();
            string sql = @" SELECT `Source`,
                                `LogLevel`,
                                `Msg`,
                                `DetailTrace`,
                                `Path`,
                                `Method`,
                                `RequestInfo`,
                                `ResponseTime`,
                                `LogTime`
                            FROM `app_log`
                            {0} 
                            ORDER BY
	                            `Id` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.Path))
            {
                where += " AND `Path` LIKE '%" + searchParam.Path + "%'";
            }
            if (!string.IsNullOrEmpty(searchParam.Level))
            {
                where += " AND `LogLevel` = @Level";
                param.Add(new MySqlParameter("@Level", searchParam.Level));
            }
            if (!string.IsNullOrEmpty(searchParam.SearchKey))
            {
                where += " AND `Msg` LIKE '%" + searchParam.SearchKey + "%'";
            }
            if (!string.IsNullOrEmpty(searchParam.StartDate))
            {
                where += " AND `LogTime` >= @StartDate";
                param.Add(new MySqlParameter("@StartDate", Convert.ToDateTime(searchParam.StartDate)));
            }
            if (!string.IsNullOrEmpty(searchParam.EndDate))
            {
                where += " AND `LogTime` < @EndDate";
                param.Add(new MySqlParameter("@EndDate", Convert.ToDateTime(searchParam.EndDate).AddDays(1)));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ApiLog log = new ApiLog()
                    {
                        Source = Converter.TryToString(row["Source"]),
                        LogLevel = Converter.TryToString(row["LogLevel"]),
                        Msg = Converter.TryToString(row["Msg"]),
                        DetailTrace = Converter.TryToString(row["DetailTrace"]),
                        Path = Converter.TryToString(row["Path"]),
                        Method = Converter.TryToString(row["Method"]),
                        RequestInfo = Converter.TryToString(row["RequestInfo"]),
                        ResponseTime = Converter.TryToString(row["ResponseTime"]),
                        LogTime = Converter.TryToDateTime(row["LogTime"]).ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    if (log.LogLevel == "ERROR")
                    {
                        log.RequestInfo += $"异常：{log.Msg}\r\n{log.DetailTrace}";
                    }
                    else
                    {
                        log.RequestInfo += $"消息：{log.Msg}";
                    }
                    log.RequestInfo += $"\r\n请求方式：{log.Method}";
                    log.RequestInfo += $"\r\n耗时：{log.ResponseTime} 毫秒";

                    recordList.Add(log);
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `app_log`
                                {0} ";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }
    }
}
