using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class ApplyFormBLL
    {
        internal static List<PartnerFormGuide> GetGuideList(PartnerFormParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<PartnerFormGuide> recordList = new List<PartnerFormGuide>();
            string sql = @" SELECT f.`ID`,
                                f.`ContactName`,
                                f.`ContactPhone`,
                                f.`IDCard`,
                                f.`Attachment`,
                                f.`OpenID`,
                                f.`Status`,
                                f.`PayStatus`,
                                f.`BindPartnerUserID`,
                                f.`CreateTime`,
                                f.`UpdateTime`,
                                e.`ChineseName` as `UpdateBy`
                            FROM `mt_partner_form_guide` f
                            LEFT JOIN mt_employee e ON f.UpdateBy = e.UserName
                            {0} 
                            ORDER BY
	                            f.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.KeyWords))
            {
                where += " AND (f.`ContactName` like CONCAT('%',@KeyWords,'%') OR f.`ContactPhone` like CONCAT('%',@KeyWords,'%') OR f.`IDCard` like CONCAT('%',@KeyWords,'%'))";
                param.Add(new MySqlParameter("@KeyWords", searchParam.KeyWords));
            }
            if (searchParam.Status != null)
            {
                where += " AND f.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", (int)searchParam.Status));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new PartnerFormGuide()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        ContactName = Converter.TryToString(row["ContactName"]),
                        ContactPhone = Converter.TryToString(row["ContactPhone"]),
                        IDCard = Converter.TryToString(row["IDCard"]),
                        Attachment = Converter.TryToString(row["Attachment"]),
                        OpenID = Converter.TryToString(row["OpenID"]),
                        Status = Converter.TryToInt32(row["Status"], -1),
                        PayStatus = Converter.TryToInt32(row["PayStatus"], -1),
                        BindPartnerUserID = Converter.TryToString(row["BindPartnerUserID"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = Converter.TryToDateTime(row["UpdateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateBy = Converter.TryToString(row["UpdateBy"]),
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_partner_form_guide` f
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static List<PartnerFormGeneral> GetGeneralList(PartnerFormParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<PartnerFormGeneral> recordList = new List<PartnerFormGeneral>();
            string sql = @" SELECT f.`ID`,
                                f.`MerchantType`,
                                f.`MerchantName`,
                                f.`ContactName`,
                                f.`ContactPhone`,
                                f.`MerchantAddress`,
                                f.`MerchantScale`,
                                f.`BusinessType`,
                                f.`IsChain`,
                                f.`OpenID`,
                                f.`Status`,
                                f.`PayStatus`,
                                f.`BindPartnerUserID`,
                                f.`CreateTime`,
                                f.`UpdateTime`,
                                e.`ChineseName` as `UpdateBy`
                            FROM `mt_partner_form_general` f
                            LEFT JOIN mt_employee e ON f.UpdateBy = e.UserName
                            {0} 
                            ORDER BY
	                            f.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.KeyWords))
            {
                where += " AND (f.`ContactName` like CONCAT('%',@KeyWords,'%') OR f.`ContactPhone` like CONCAT('%',@KeyWords,'%') OR f.`IDCard` like CONCAT('%',@KeyWords,'%'))";
                param.Add(new MySqlParameter("@KeyWords", searchParam.KeyWords));
            }
            if (searchParam.Status != null)
            {
                where += " AND f.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", (int)searchParam.Status));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new PartnerFormGeneral()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        MerchantType = Converter.TryToString(row["MerchantType"]),
                        MerchantName = Converter.TryToString(row["MerchantName"]),
                        ContactName = Converter.TryToString(row["ContactName"]),
                        ContactPhone = Converter.TryToString(row["ContactPhone"]),
                        MerchantAddress = Converter.TryToString(row["MerchantAddress"]),
                        MerchantScale = Converter.TryToString(row["MerchantScale"]),
                        BusinessType = Converter.TryToString(row["BusinessType"]),
                        IsChain = Converter.TryToString(row["IsChain"]),
                        OpenID = Converter.TryToString(row["OpenID"]),
                        Status = Converter.TryToInt32(row["Status"], -1),
                        PayStatus = Converter.TryToInt32(row["PayStatus"], -1),
                        BindPartnerUserID = Converter.TryToString(row["BindPartnerUserID"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = Converter.TryToDateTime(row["UpdateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateBy = Converter.TryToString(row["UpdateBy"]),
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_partner_form_general` f
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static List<PartnerFormAdvanced> GetAdvancedList(PartnerFormParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<PartnerFormAdvanced> recordList = new List<PartnerFormAdvanced>();
            string sql = @" SELECT f.`ID`,
                                f.`MerchantType`,
                                f.`MerchantName`,
                                f.`ContactName`,
                                f.`ContactPhone`,
                                f.`MerchantAddress`,
                                f.`MerchantScale`,
                                f.`BusinessType`,
                                f.`IsChain`,
                                f.`OpenID`,
                                f.`Status`,
                                f.`PayStatus`,
                                f.`BindPartnerUserID`,
                                f.`CreateTime`,
                                f.`UpdateTime`,
                                e.`ChineseName` as `UpdateBy`
                            FROM `mt_partner_form_advanced` f
                            LEFT JOIN mt_employee e ON f.UpdateBy = e.UserName
                            {0} 
                            ORDER BY
	                            f.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.KeyWords))
            {
                where += " AND (f.`ContactName` like CONCAT('%',@KeyWords,'%') OR f.`ContactPhone` like CONCAT('%',@KeyWords,'%') OR f.`IDCard` like CONCAT('%',@KeyWords,'%'))";
                param.Add(new MySqlParameter("@KeyWords", searchParam.KeyWords));
            }
            if (searchParam.Status != null)
            {
                where += " AND f.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", (int)searchParam.Status));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new PartnerFormAdvanced()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        MerchantType = Converter.TryToString(row["MerchantType"]),
                        MerchantName = Converter.TryToString(row["MerchantName"]),
                        ContactName = Converter.TryToString(row["ContactName"]),
                        ContactPhone = Converter.TryToString(row["ContactPhone"]),
                        MerchantAddress = Converter.TryToString(row["MerchantAddress"]),
                        MerchantScale = Converter.TryToString(row["MerchantScale"]),
                        BusinessType = Converter.TryToString(row["BusinessType"]),
                        IsChain = Converter.TryToString(row["IsChain"]),
                        OpenID = Converter.TryToString(row["OpenID"]),
                        Status = Converter.TryToInt32(row["Status"], -1),
                        PayStatus = Converter.TryToInt32(row["PayStatus"], -1),
                        BindPartnerUserID = Converter.TryToString(row["BindPartnerUserID"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = Converter.TryToDateTime(row["UpdateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateBy = Converter.TryToString(row["UpdateBy"]),
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_partner_form_advanced` f
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static bool Approve(PartnerApproveData data)
        {
            string tableName;
            switch (data.DataType)
            {
                case "touristguide": tableName = "mt_partner_form_guide"; break;
                case "general": tableName = "mt_partner_form_general"; break;
                case "advanced": tableName = "mt_partner_form_advanced"; break;
                default: throw new MsgException("参数错误！");
            }

            // 查询用户是否存在
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                 "select count(*) from mt_partner where UserName = @UserName",
                new MySqlParameter("@UserName", data.UserName));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("该账户已经存在，无需重复添加！");
            }

            // 创建用户
            string accountID = Guid.NewGuid().ToString();
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO `mt_partner`
                        (`ID`,`UserName`,`Password`,`Phone`,`Name`,`Organization`,`IsOwner`,`Type`,`CreateBy`)
                    VALUES
                        (@ID,@UserName,@Password,@Phone,@Name,@Organization,@IsOwner,@Type,@CreateBy);",
                new MySqlParameter("@ID", accountID),
                new MySqlParameter("@UserName", data.UserName),
                new MySqlParameter("@Password", "123456"),
                new MySqlParameter("@Phone", data.Phone),
                new MySqlParameter("@Name", data.Name),
                new MySqlParameter("@Organization", data.Organization),
                new MySqlParameter("@IsOwner", 1),
                new MySqlParameter("@Type", data.DataType),
                new MySqlParameter("@CreateBy", data.UpdateBy));

            // 更新表单
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $@"Update {tableName} set Status=100,UpdateBy=@UpdateBy,BindPartnerUserID=@BindPartnerUserID
                     where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@BindPartnerUserID", accountID),
                new MySqlParameter("@UpdateBy", data.UpdateBy));

            return true;
        }
        internal static bool Reject(PartnerApproveData data)
        {
            string tableName;
            switch (data.DataType)
            {
                case "touristguide": tableName = "mt_partner_form_guide"; break;
                case "general": tableName = "mt_partner_form_general"; break;
                case "advanced": tableName = "mt_partner_form_advanced"; break;
                default: throw new MsgException("参数错误！");
            }

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $@"Update {tableName} set Status=200,UpdateBy=@UpdateBy
                     where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }
    }
}
