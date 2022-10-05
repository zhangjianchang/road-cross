using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class CouponsBLL
    {
        internal static List<Coupons> GetList(CouponsParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<Coupons> recordList = new List<Coupons>();
            string sql = @" SELECT `ID`,
                                `Name`,
                                `Title`,
                                `SubTitle`,
                                `StartTime`,
                                `EndTime`,
                                `IsAutoGive`,
                                `RuleType`,
                                `Threshold`,
                                `Discount`,
                                `CreateTime`,
                                 e.`ChineseName` as `CreateBy`
                            FROM `mt_coupons` c
                            LEFT JOIN `mt_employee` e
                            ON c.`CreateBy` = e.`UserName`
                            {0} 
                            ORDER BY
	                            c.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();
            if (!string.IsNullOrEmpty(searchParam.KeyWords))
            {
                where += " AND (c.`Name` LIKE '%" + searchParam.KeyWords + "%' OR c.`Title` LIKE '%" + searchParam.KeyWords + "%') ";
            }
            if (searchParam.Status != null)
            {
                if (searchParam.Status == 1)
                {
                    // 可用
                    where += $" AND now()>=c.`StartTime` AND now()<=c.`EndTime` ";
                }
                else
                {
                    // 不可用
                    where += $" AND (now()<c.`StartTime` OR now()>c.`EndTime`) ";
                }
            }
            if (searchParam.RuleType != null)
            {
                where += " AND c.`RuleType` = @RuleType";
                param.Add(new MySqlParameter("@RuleType", searchParam.RuleType));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Coupons item = new Coupons()
                    {
                        ID = Converter.TryToInt64(row["ID"]),
                        Name = Converter.TryToString(row["Name"]),
                        Title = Converter.TryToString(row["Title"]),
                        SubTitle = Converter.TryToString(row["SubTitle"]),
                        StartTime = Converter.TryToDateTime(row["StartTime"]).ToString("yyyy-MM-dd"),
                        EndTime = Converter.TryToDateTime(row["EndTime"]).ToString("yyyy-MM-dd"),
                        IsAutoGive = Converter.TryToInt32(row["IsAutoGive"]),
                        RuleType = Converter.TryToInt32(row["RuleType"]),
                        Threshold = Converter.TryToDecimal(row["Threshold"]),
                        Discount = Converter.TryToDecimal(row["Discount"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        CreateBy = Converter.TryToString(row["CreateBy"])
                    };
                    if (item.RuleType == 2)
                    {
                        item.Discount = item.Discount * 10;
                    }
                    recordList.Add(item);
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_coupons` c
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static bool UpdateCoupons(Coupons data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"Update `mt_coupons` 
                    set Name=@Name,
                        Title=@Title,
                        SubTitle=@SubTitle,
                        StartTime=@StartTime,
                        EndTime=@EndTime,
                        IsAutoGive=@IsAutoGive,
                        RuleType=@RuleType,
                        Threshold=@Threshold,
                        Discount=@Discount,
                        UpdateTime=@UpdateTime,
                        UpdateBy =@UpdateBy
                    where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@Name", data.Name),
                new MySqlParameter("@Title", data.Title),
                new MySqlParameter("@SubTitle", data.SubTitle),
                new MySqlParameter("@StartTime", data.StartTime),
                new MySqlParameter("@EndTime", data.EndTime),
                new MySqlParameter("@IsAutoGive", data.IsAutoGive),
                new MySqlParameter("@RuleType", data.RuleType),
                new MySqlParameter("@Threshold", data.Threshold),
                new MySqlParameter("@Discount", data.Discount),
                new MySqlParameter("@UpdateTime", DateTime.Now),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }

        internal static bool AddCoupons(Coupons data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO `mt_coupons`(
                        `Name`,
                        `Title`,
                        `SubTitle`,
                        `StartTime`,
                        `EndTime`,
                        `IsAutoGive`,
                        `RuleType`,
                        `Threshold`,
                        `Discount`,
                        `CreateTime`,
                        `CreateBy`)
                       VALUES(
                        @Name,
                        @Title,
                        @SubTitle,
                        @StartTime,
                        @EndTime,
                        @IsAutoGive,
                        @RuleType,
                        @Threshold,
                        @Discount,
                        @CreateTime,
                        @CreateBy);",
                new MySqlParameter("@Name", data.Name),
                new MySqlParameter("@Title", data.Title),
                new MySqlParameter("@SubTitle", data.SubTitle),
                new MySqlParameter("@StartTime", data.StartTime),
                new MySqlParameter("@EndTime", data.EndTime),
                new MySqlParameter("@IsAutoGive", data.IsAutoGive),
                new MySqlParameter("@RuleType", data.RuleType),
                new MySqlParameter("@Threshold", data.Threshold),
                new MySqlParameter("@Discount", data.Discount),
                new MySqlParameter("@CreateTime", DateTime.Now),
                new MySqlParameter("@CreateBy", data.CreateBy));
            return true;
        }

        internal static List<CouponsCustomer> GetCustomerOptions(CouponsCustomerParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<CouponsCustomer> recordList = new List<CouponsCustomer>();
            string sql = @" SELECT * FROM (
							    SELECT `OpenID`,
                                    `Telphone`,
                                    `RegistTime`,
                                    `MemberType`,
                                    (select count(id) from mt_coupons_customer where OpenId=p.OpenID and CouponID=@CouponID) as IsSend
                                FROM `mt_customer` p                            
                                {0} 
                            ) as t 
                            {1}
                            ORDER BY
	                            t.`RegistTime` DESC
                            LIMIT {2},{3} ";

            string where1 = " WHERE 1=1 ";
            string where2 = "";
            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@CouponID", searchParam.CouponID));
            if (!string.IsNullOrEmpty(searchParam.Telphone))
            {
                where1 += " AND p.`Telphone`=@Telphone";
                param.Add(new MySqlParameter("@Telphone", searchParam.Telphone));
            }
            if (!string.IsNullOrEmpty(searchParam.MemberType))
            {
                where1 += " AND p.`MemberType` = @MemberType";
                param.Add(new MySqlParameter("@MemberType", searchParam.MemberType));
            }
            if (!string.IsNullOrEmpty(searchParam.RegistTimeStart))
            {
                where1 += " AND p.RegistTime > @RegistTimeStart";
                param.Add(new MySqlParameter("@RegistTimeStart", Convert.ToDateTime(searchParam.RegistTimeStart)));
            }
            if (!string.IsNullOrEmpty(searchParam.RegistTimeEnd))
            {
                where1 += " AND p.RegistTime < @RegistTimeEnd";
                param.Add(new MySqlParameter("@RegistTimeEnd", Convert.ToDateTime(searchParam.RegistTimeEnd)));
            }
            if (searchParam.IsSend != null)
            {
                where2 += " WHERE t.IsSend " + (searchParam.IsSend == 0 ? "<" : ">=") + " 1";
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where1, where2, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new CouponsCustomer()
                    {
                        OpenID = Converter.TryToString(row["OpenID"]),
                        Telphone = Converter.TryToString(row["Telphone"]),
                        RegistTime = Converter.TryToDateTime(row["RegistTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        MemberType = Converter.TryToString(row["MemberType"]),
                        IsSend = Converter.TryToInt32(row["IsSend"])
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT count(*) FROM (
							        SELECT `OpenID`,
                                        `Telphone`,
                                        `RegistTime`,
                                        `MemberType`,
                                        `Status`,
                                        (select count(id) from mt_coupons_customer where OpenId=p.OpenID and CouponID=1) as IsSend
                                    FROM `mt_customer` p                            
                                    {0} 
                                ) as t 
                                {1}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where1, where2), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }


        internal static bool SendBatch(CouponsCustomerParam data)
        {
            string sql = @" INSERT INTO `mt_coupons_customer`
	                            (`CouponID`,`OpenID`,`Status`,`CreateBy`)
                            SELECT @CouponID,`OpenID`,100,'' 
                            FROM (
	                            SELECT `OpenID`,
		                            (select count(id) from mt_coupons_customer where OpenId=p.OpenID and CouponID=@CouponID) as IsSend
	                            FROM `mt_customer` p                            
	                            {0} 
                            ) as t 
                            {1}";

            string where1 = " WHERE 1=1 ";
            string where2 = "";
            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@CouponID", data.CouponID));
            if (!string.IsNullOrEmpty(data.Telphone))
            {
                where1 += " AND p.`Telphone`=@Telphone";
                param.Add(new MySqlParameter("@Telphone", data.Telphone));
            }
            if (!string.IsNullOrEmpty(data.MemberType))
            {
                where1 += " AND p.`MemberType` = @MemberType";
                param.Add(new MySqlParameter("@MemberType", data.MemberType));
            }
            if (!string.IsNullOrEmpty(data.RegistTimeStart))
            {
                where1 += " AND p.RegistTime > @RegistTimeStart";
                param.Add(new MySqlParameter("@RegistTimeStart", Convert.ToDateTime(data.RegistTimeStart)));
            }
            if (!string.IsNullOrEmpty(data.RegistTimeEnd))
            {
                where1 += " AND p.RegistTime < @RegistTimeEnd";
                param.Add(new MySqlParameter("@RegistTimeEnd", Convert.ToDateTime(data.RegistTimeEnd)));
            }
            if (data.IsSend != null)
            {
                where2 += " WHERE t.IsSend " + (data.IsSend == 0 ? "<" : ">=") + " 1";
            }

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection, string.Format(sql, where1, where2), param.ToArray());
            return true;
        }
        internal static bool SendSingle(CouponsCustomerParam data)
        {
            string sql = @" INSERT INTO `mt_coupons_customer`
	                            (`CouponID`,`OpenID`,`Status`,`CreateBy`)
                            VALUES(@CouponID,@OpenID,100,@CreateBy)";
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                sql,
                new MySqlParameter("@CouponID", data.CouponID),
                new MySqlParameter("@OpenID", data.OpenID),
                new MySqlParameter("@CreateBy", data.CreateBy));
            return true;
        }

        internal static List<CouponsCustomer> GetCouponsCustomer(CouponsCustomerParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<CouponsCustomer> recordList = new List<CouponsCustomer>();
            string sql = @"SELECT r.`ID`, 
                                r.`Status`, 
								r.`GiveTime`,
                                r.`UseTime`,
                                cp.`Name`,
                                ct.`Telphone`,
                                ct.`MemberType`,
								ct.`RegistTime`
                            FROM `mt_coupons_customer` r
                            INNER JOIN `mt_coupons` cp ON r.`CouponID`=cp.`ID`
                            INNER JOIN `mt_customer` ct ON r.`OpenID`=ct.`OpenID`                          
                                {0}
                            ORDER BY
	                            r.`GiveTime` DESC
                            LIMIT {1},{2}";

            string where = " WHERE 1=1 ";
            List<MySqlParameter> param = new List<MySqlParameter>();
            if (!string.IsNullOrEmpty(searchParam.Telphone))
            {
                where += " AND ct.`Telphone` = @Telphone";
                param.Add(new MySqlParameter("@Telphone", searchParam.Telphone));
            }
            if (searchParam.CouponID != null)
            {
                where += " AND r.`CouponID` = @CouponID";
                param.Add(new MySqlParameter("@CouponID", searchParam.CouponID));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    CouponsCustomer item = new CouponsCustomer()
                    {
                        ID = Converter.TryToInt64(row["ID"]),
                        Status = Converter.TryToInt32(row["Status"]),
                        GiveTime = Converter.TryToDateTime(row["GiveTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UseTime = Converter.TryToDateTime(row["UseTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        CouponName = Converter.TryToString(row["Name"]),
                        Telphone = Converter.TryToString(row["Telphone"]),
                        MemberType = Converter.TryToString(row["MemberType"]),
                        RegistTime = Converter.TryToDateTime(row["RegistTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                    };
                    recordList.Add(item);
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM `mt_coupons_customer` r
                                INNER JOIN `mt_coupons` cp ON r.`CouponID`=cp.`ID`
                                INNER JOIN `mt_customer` ct ON r.`OpenID`=ct.`OpenID`      
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }
    }

}
