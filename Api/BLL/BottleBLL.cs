using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class BottleBLL
    {
        internal static List<Bottle> GetBottleList(BottleParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<Bottle> recordList = new List<Bottle>();
            string sql = @" SELECT
	                            b.`ID`,
	                            b.`Code`,
	                            b.`RFID`,
	                            b.`Spec`,
	                            t.`Name` as `SpecName`,
	                            b.`Status`,
	                            b.`PurchaseTime`,
	                            b.`PurchasePerson`,
	                            b.`QRCode`,
	                            b.`CreateTime`,
                                e.`ChineseName`,
                                gb.`GridID`,
                                g.`GridNumber`,
                                c.`Number` AS CabinetNumber,
                                c.`Name` AS CabinetName
                            FROM
	                            mt_bottle b
	                            LEFT JOIN mt_employee e ON b.CreateBy = e.UserName 
	                            LEFT JOIN mt_grid_bottle gb ON gb.BottleID = b.ID
	                            LEFT JOIN mt_grid g ON g.ID = gb.GridID
	                            LEFT JOIN mt_cabinet c ON c.ID= g.CabinetID
	                            LEFT JOIN mt_bottletype t ON t.ID= b.Spec
                            {0} 
                            ORDER BY
	                            b.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE b.IsDeleted = 0 ";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.Code))
            {
                where += " AND b.`Code` = @Code";
                param.Add(new MySqlParameter("@Code", searchParam.Code));
            }
            if (!string.IsNullOrEmpty(searchParam.Status))
            {
                where += " AND b.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", searchParam.Status));
            }
            if (!string.IsNullOrEmpty(searchParam.Spec))
            {
                where += " AND b.`Spec` = @Spec";
                param.Add(new MySqlParameter("@Spec", searchParam.Spec));
            }
            if (!string.IsNullOrEmpty(searchParam.NotBind))
            {
                where += " AND gb.`GridID` IS NULL";
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Bottle()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        Code = Converter.TryToString(row["Code"]),
                        RFID = Converter.TryToString(row["RFID"]),
                        Spec = Converter.TryToString(row["Spec"]),
                        SpecName = Converter.TryToString(row["SpecName"]),
                        Status = Converter.TryToString(row["Status"]),
                        PurchaseTime = Converter.TryToDateTime(row["PurchaseTime"]).ToString("yyyy-MM-dd"),
                        PurchasePerson = Converter.TryToString(row["PurchasePerson"]),
                        QRCode = Converter.TryToString(row["QRCode"]),
                        CreateTime = Converter.TryToString(row["CreateTime"]),
                        CreateBy = Converter.TryToString(row["ChineseName"]),
                        GridID = Converter.TryToString(row["GridID"]),
                        GridNumber = Converter.TryToString(row["GridNumber"]),
                        CabinetName = Converter.TryToString(row["CabinetName"]),
                        CabinetNumber = Converter.TryToString(row["CabinetNumber"])
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM
	                                mt_bottle b
	                                LEFT JOIN mt_grid_bottle gb ON gb.BottleID = b.ID
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static bool UpdateBottle(Bottle data)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                 "select count(*) from mt_bottle where IsDeleted=0 AND RFID = @RFID AND ID<>@ID;",
                             new MySqlParameter("@RFID", data.RFID),
                             new MySqlParameter("@ID", data.ID));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("RFID已存在！");
            }

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"Update mt_bottle set 
                        RFID=@RFID
                        ,Spec=@Spec
                        ,Status=@Status
                        ,PurchaseTime=@PurchaseTime
                        ,PurchasePerson=@PurchasePerson
                        ,UpdateBy=@UpdateBy
                     where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@RFID", data.RFID),
                new MySqlParameter("@Spec", data.Spec),
                new MySqlParameter("@Status", data.Status),
                new MySqlParameter("@PurchaseTime", data.PurchaseTime),
                new MySqlParameter("@PurchasePerson", data.PurchasePerson),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }

        internal static bool AddNewBottle(Bottle data)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                 "select count(*) from mt_bottle where IsDeleted=0 AND (Code=@Code OR RFID = @RFID);",
                             new MySqlParameter("@Code", data.Code),
                             new MySqlParameter("@RFID", data.RFID));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("瓶子编号或RFID已存在！");
            }

            // 生成二维码图片

            data.QRCode = QRCodeHelper.Generator(data.QRCodeContent, data.Code);

            data.CreateBy = data.UpdateBy;
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO mt_bottle (
                        `ID`				
                        ,`Code`				
                        ,`RFID`			
                        ,`Spec`			
                        ,`Status`			
                        ,`PurchaseTime`	
                        ,`PurchasePerson`	
                        ,`QRCode`			
                        ,`CreateBy`	
                    ) VALUES (
                        @ID				
                        ,@Code				
                        ,@RFID		
                        ,@Spec			
                        ,@Status			
                        ,@PurchaseTime	
                        ,@PurchasePerson	
                        ,@QRCode				
                        ,@CreateBy);",
                new MySqlParameter("@ID", Guid.NewGuid().ToString()),
                new MySqlParameter("@Code", data.Code),
                new MySqlParameter("@RFID", data.RFID),
                new MySqlParameter("@Spec", data.Spec),
                new MySqlParameter("@Status", data.Status),
                new MySqlParameter("@PurchaseTime", Converter.TryToDateTime(data.PurchaseTime).Date),
                new MySqlParameter("@PurchasePerson", data.PurchasePerson),
                new MySqlParameter("@QRCode", data.QRCode),
                new MySqlParameter("@CreateBy", data.CreateBy));
            return true;
        }

        internal static bool DeleteById(Bottle data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Update mt_bottle SET IsDeleted=1,DeletedBy=@DeletedBy where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@DeletedBy", data.DeletedBy));
            return true;
        }

        internal static Bottle GetById(string id)
        {
            string sql = @" SELECT
	                            b.`ID`,
	                            b.`Code`,
	                            b.`Spec`,
	                            b.`Status`,
	                            b.`PurchaseTime`,
	                            b.`PurchasePerson`,
	                            b.`QRCode`,
	                            b.`CreateTime`
                            FROM
	                            mt_bottle b
                            WHERE ID=@ID
                                AND b.IsDeleted = 0 ";

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, new MySqlParameter("@ID", id));

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Bottle()
                {
                    ID = Converter.TryToString(row["ID"]),
                    Code = Converter.TryToString(row["Code"]),
                    Spec = Converter.TryToString(row["Spec"]),
                    Status = Converter.TryToString(row["Status"]),
                    PurchaseTime = Converter.TryToDateTime(row["PurchaseTime"]).ToString("yyyy-MM-dd"),
                    PurchasePerson = Converter.TryToString(row["PurchasePerson"]),
                    QRCode = Converter.TryToString(row["QRCode"]),
                    CreateTime = Converter.TryToString(row["CreateTime"])
                };
            }

            return null;
        }

        internal static bool ReGenerateQRCode(Bottle data)
        {
            data.QRCode = QRCodeHelper.Generator(data.QRCodeContent, data.Code);
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"Update mt_bottle set 
                        QRCode=@QRCode
                        ,UpdateBy=@UpdateBy
                     where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@QRCode", data.QRCode),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }
        internal static bool GenerateQRCodeForEmpty(string updateBy)
        {
            // 查询所有二维码为空的数据
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, "SELECT ID,Code FROM mt_bottle WHERE QRCode is null");

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ReGenerateQRCode(new Bottle()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        Code = Converter.TryToString(row["Code"]),
                        UpdateBy = updateBy
                    });
                }
            }
            return true;
        }
    }
}
