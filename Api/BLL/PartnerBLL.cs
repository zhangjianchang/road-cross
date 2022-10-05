using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class PartnerBLL
    {
        internal static List<Partner> GetList(PartnerParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<Partner> recordList = new List<Partner>();
            string sql = @" SELECT `ID`,
                                `UserName`,
                                `Phone`,
                                `Name`,
                                `Organization`,
                                `IsOwner`,
                                `OwnerID`,
                                `Type`,
                                `Status`,
                                (select count(ID) from mt_cabinet where BindPartnerID=p.ID) as BindBoxCount,
                                `CreateTime`
                            FROM `mt_partner` p
                            {0} 
                            ORDER BY
	                            p.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE `Type`=@Type ";
            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@Type", searchParam.Type));

            if (!string.IsNullOrEmpty(searchParam.KeyWords))
            {
                where += " AND (p.`Name` like CONCAT('%',@KeyWords,'%') OR p.`Phone` like CONCAT('%',@KeyWords,'%'))";
                param.Add(new MySqlParameter("@KeyWords", searchParam.KeyWords));
            }
            if (searchParam.Status != null)
            {
                where += " AND p.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", (int)searchParam.Status));
            }
            if (!string.IsNullOrEmpty(searchParam.Organization))
            {
                where += " AND p.`Organization` = @Organization";
                param.Add(new MySqlParameter("@Organization", searchParam.Organization));
            }
            if (searchParam.IsOwner != null)
            {
                where += " AND p.`IsOwner` = @IsOwner";
                param.Add(new MySqlParameter("@IsOwner", (int)searchParam.IsOwner));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Partner()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        UserName = Converter.TryToString(row["UserName"]),
                        Phone = Converter.TryToString(row["Phone"]),
                        Name = Converter.TryToString(row["Name"]),
                        Organization = Converter.TryToString(row["Organization"]),
                        IsOwner = Converter.TryToInt32(row["IsOwner"]),
                        OwnerID = Converter.TryToString(row["OwnerID"]),
                        Type = Converter.TryToString(row["Type"]),
                        Status = Converter.TryToInt32(row["Status"]),
                        BindBoxCount = Converter.TryToInt32(row["BindBoxCount"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_partner` p
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static List<Cabinet> GetBindBoxList(string id)
        {
            List<Cabinet> recordList = new List<Cabinet>();
            string sql = @" SELECT c.`Name`										
                                ,c.`Number`									
                                ,c.`Province`									
                                ,c.`City`										
                                ,c.`Country`									
                                ,c.`Address`
                                ,c.`DeviceCode`									
                                ,c.`DeviceStatus`								
                                ,c.`Status`		
                            FROM mt_cabinet c
                            where BindPartnerID=@BindPartnerID
                            ORDER by c.`Name` DESC";
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, new MySqlParameter("@BindPartnerID", id));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Cabinet()
                    {
                        Province = Converter.TryToString(row["Province"]),
                        City = Converter.TryToString(row["City"]),
                        Country = Converter.TryToString(row["Country"]),
                        Number = Converter.TryToString(row["Number"]),
                        Name = Converter.TryToString(row["Name"]),
                        DeviceCode = Converter.TryToString(row["DeviceCode"]),
                        Address = Converter.TryToString(row["Address"]),
                        Status = Converter.TryToString(row["Status"])
                    });
                }
            }

            return recordList;
        }

        internal static bool UpdateStatus(Partner data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                       $@"Update mt_partner set Status=@Status,UpdateBy=@UpdateBy
                     where ID=@ID;",
                   new MySqlParameter("@ID", data.ID),
                   new MySqlParameter("@Status", data.Status),
                   new MySqlParameter("@UpdateBy", data.UpdateBy));

            return true;
        }
        internal static bool ResetPassword(Partner data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                       $@"Update mt_partner set Password='123456',UpdateBy=@UpdateBy
                     where ID=@ID;",
                   new MySqlParameter("@ID", data.ID),
                   new MySqlParameter("@UpdateBy", data.UpdateBy));

            return true;
        }

        #region 提现

        internal static List<Partner> WithdrawList(PartnerParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<Partner> recordList = new List<Partner>();
            string sql = @" SELECT w.`ID`,
                                p.`Name`,
                                p.`Phone`,
                                p.`Type`,
                                w.`Amount`,
                                w.`Status`,
                                w.`CreateTime`,
                                w.`UpdateTime`,
                                w.`UpdateBy`
                            FROM `mt_partner` p
                            INNER JOIN `mt_partner_withdraw` w
                            ON p.`ID` = w.`PartnerID`
                            {0} 
                            ORDER BY
	                            w.`UpdateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.KeyWords))
            {
                where += " AND (p.`Name` like CONCAT('%',@KeyWords,'%') OR p.`Phone` like CONCAT('%',@KeyWords,'%'))";
                param.Add(new MySqlParameter("@KeyWords", searchParam.KeyWords));
            }
            if (searchParam.Status != null)
            {
                where += " AND w.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", (int)searchParam.Status));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Partner()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        Phone = Converter.TryToString(row["Phone"]),
                        Name = Converter.TryToString(row["Name"]),
                        Type = Converter.TryToString(row["Type"]),
                        Amount = Converter.TryToString(row["Amount"]),
                        Status = Converter.TryToInt32(row["Status"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = Converter.TryToDateTime(row["UpdateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateBy = Converter.TryToString(row["UpdateBy"]),
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_partner` p
                                INNER JOIN `mt_partner_withdraw` w
                                ON p.`ID` = w.`PartnerID`
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static bool UpdateWithdrawStatus(Partner data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                       $@"Update mt_partner_withdraw set Status=@Status,UpdateBy=@UpdateBy
                     where ID=@ID;",
                   new MySqlParameter("@ID", data.ID),
                   new MySqlParameter("@Status", data.Status),
                   new MySqlParameter("@UpdateBy", data.UpdateBy));

            return true;
        }

        #endregion
    }
}
