using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class CustomerBLL
    {
        internal static List<Customer> GetList(CustomerParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<Customer> recordList = new List<Customer>();
            string sql = @" SELECT `OpenID`,
                                `Telphone`,
                                `RegistTime`,
                                `MemberType`,
                                (select sum(Points) as total from mt_userpointsdetail where OpenId=p.OpenID) as `MemberPoints`,
                                `Status`,
                                (select count(OrderNo) from mt_order_memberlevel where OpenId=p.OpenID and PayStatus=200) as LevelupRecordCount
                            FROM `mt_customer` p
                            {0} 
                            ORDER BY
	                            p.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();
            if (!string.IsNullOrEmpty(searchParam.Telphone))
            {
                where += " AND p.`Telphone`=@Telphone";
                param.Add(new MySqlParameter("@Telphone", searchParam.Telphone));
            }
            if (searchParam.Status != null)
            {
                where += " AND p.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", searchParam.Status));
            }
            if (!string.IsNullOrEmpty(searchParam.MemberType))
            {
                where += " AND p.`MemberType` = @MemberType";
                param.Add(new MySqlParameter("@MemberType", searchParam.MemberType));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Customer()
                    {
                        OpenID = Converter.TryToString(row["OpenID"]),
                        Telphone = Converter.TryToString(row["Telphone"]),
                        RegistTime = Converter.TryToDateTime(row["RegistTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        MemberType = Converter.TryToString(row["MemberType"]),
                        MemberPoints = Converter.TryToDecimal(row["MemberPoints"]),
                        Status = Converter.TryToInt32(row["Status"]),
                        LevelupRecordCount = Converter.TryToInt32(row["LevelupRecordCount"])
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_customer` p
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static List<PointsRecord> GetPointsList(CustomerParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<PointsRecord> recordList = new List<PointsRecord>();
            string sql = @" SELECT p.`Telphone`,
                                d.`Points`,
                                d.`Remark`,
                                d.`ScoreWay`,
                                d.`OrderNo`,
                                d.`CreateTime`
                            FROM `mt_userpointsdetail` d 
                            INNER JOIN `mt_customer` p ON d.OpenID = p.OpenID
                            {0} 
                            ORDER BY
	                            d.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();
            if (!string.IsNullOrEmpty(searchParam.Telphone))
            {
                where += " AND p.`Telphone`=@Telphone";
                param.Add(new MySqlParameter("@Telphone", searchParam.Telphone));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new PointsRecord()
                    {
                        Telphone = Converter.TryToString(row["Telphone"]),
                        Points = Converter.TryToString(row["Points"]),
                        Remark = Converter.TryToString(row["Remark"]),
                        ScoreWay = Converter.TryToString(row["ScoreWay"]),
                        OrderNo = Converter.TryToString(row["OrderNo"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_userpointsdetail` d 
                                INNER JOIN `mt_customer` p ON d.OpenID = p.OpenID
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static List<LevelupRecord> GetLevelupRecord(string id)
        {
            List<LevelupRecord> recordList = new List<LevelupRecord>();
            string sql = @" SELECT c.`OrderNo`										
                                ,c.`TargetLevel`										
                                ,c.`CurrentLevel`										
                                ,c.`PayWay`										
                                ,c.`TotalAmount`										
                                ,c.`PayStatus`										
                                ,c.`CreateTime`
                            FROM mt_order_memberlevel c
                            where OpenId=@OpenId
                            ORDER by c.`CreateTime` DESC";
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, new MySqlParameter("@OpenId", id));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new LevelupRecord()
                    {
                        OrderNo = Converter.TryToString(row["OrderNo"]),
                        TargetLevel = Converter.TryToString(row["TargetLevel"]),
                        CurrentLevel = Converter.TryToString(row["CurrentLevel"]),
                        PayWay = Converter.TryToString(row["PayWay"]),
                        TotalAmount = Converter.TryToDecimal(row["TotalAmount"]),
                        PayStatus = Converter.TryToString(row["PayStatus"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }
            }

            return recordList;
        }

        internal static bool UpdateStatus(Customer data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                       $@"Update mt_customer set Status=@Status,UpdateBy=@UpdateBy
                     where OpenID=@OpenID;",
                   new MySqlParameter("@OpenID", data.OpenID),
                   new MySqlParameter("@Status", data.Status),
                   new MySqlParameter("@UpdateBy", data.UpdateBy));

            return true;
        }

        /// <summary>
        /// 同步老系统数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static bool SyncCustomer(Customer data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                       $@"INSERT INTO `mt_customer`(`OpenID`, `Telphone`, `RegistTime`, `MemberType`, `MemberPoints`, `Status`, `WXPayScoreState`, `WXAuthorizationCode`, `WXPayScoreOpenId`, `CreateTime`) 
                               VALUES (@OpenID, @Telphone, @RegistTime, @MemberType, @MemberPoints, @Status, @WXPayScoreState, @WXAuthorizationCode, @WXPayScoreOpenId, @CreateTime);",
                   new MySqlParameter("@OpenID", data.OpenID),
                   new MySqlParameter("@Telphone", data.Telphone),
                   new MySqlParameter("@RegistTime", data.RegistTime),
                   new MySqlParameter("@MemberType", data.MemberType),
                   new MySqlParameter("@MemberPoints", data.MemberPoints),
                   new MySqlParameter("@WXPayScoreState", data.WXPayScoreState),
                   new MySqlParameter("@WXAuthorizationCode", data.WXAuthorizationCode),
                   new MySqlParameter("@WXPayScoreOpenId", data.WXPayScoreOpenId),
                   new MySqlParameter("@CreateTime", data.RegistTime),
                   new MySqlParameter("@Status", 1));

            return true;
        }
    }
}
