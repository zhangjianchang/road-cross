using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class CabinetBLL
    {


        internal static List<Cabinet> GetCabinetList(CabinetParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<Cabinet> recordList = new List<Cabinet>();
            string sql = @" SELECT c.`ID`										
                                ,c.`Name`										
                                ,c.`Number`									
                                ,c.`Province`									
                                ,c.`City`										
                                ,c.`Country`									
                                ,c.`Address`									
                                ,c.`Longitude`									
                                ,c.`Latitude`									
                                ,c.`DeviceCode`
                                ,c.`QRCode`											
                                ,c.`DeviceStatus`								
                                ,c.`Status`								
                                ,c.`CCSStoreID`						
                                ,c.`SFStoreID`							
                                ,c.`ContactPerson`								
                                ,c.`ContactPhone`								
                                ,c.`CreateTime`								
                                ,e.ChineseName			
                            FROM mt_cabinet c
                            LEFT JOIN mt_employee e ON c.CreateBy = e.UserName 
                            {0}
                            ORDER by c.`CreateTime` DESC
                            LIMIT {1},{2} ";

            string where = " WHERE c.IsDeleted = 0 AND c.DeviceType = @DeviceType";
            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@DeviceType", searchParam.DeviceType));

            if (!string.IsNullOrEmpty(searchParam.Number))
            {
                where += " AND c.`Number` = @Number";
                param.Add(new MySqlParameter("@Number", searchParam.Number));
            }
            if (!string.IsNullOrEmpty(searchParam.ContactPerson))
            {
                where += " AND `ContactPerson` = @ContactPerson";
                param.Add(new MySqlParameter("@ContactPerson", searchParam.ContactPerson));
            }
            if (!string.IsNullOrEmpty(searchParam.ContactPhone))
            {
                where += " AND `ContactPhone` = @ContactPhone";
                param.Add(new MySqlParameter("@ContactPhone", searchParam.ContactPhone));
            }
            if (!string.IsNullOrEmpty(searchParam.Status))
            {
                where += " AND c.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", searchParam.Status));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Cabinet()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        Province = Converter.TryToString(row["Province"]),
                        City = Converter.TryToString(row["City"]),
                        Country = Converter.TryToString(row["Country"]),
                        Number = Converter.TryToString(row["Number"]),
                        Name = Converter.TryToString(row["Name"]),
                        DeviceCode = Converter.TryToString(row["DeviceCode"]),
                        Address = Converter.TryToString(row["Address"]),
                        Latitude = Converter.TryToSingle(row["Latitude"]),
                        Longitude = Converter.TryToSingle(row["Longitude"]),
                        ContactPerson = Converter.TryToString(row["ContactPerson"]),
                        ContactPhone = Converter.TryToString(row["ContactPhone"]),
                        Status = Converter.TryToString(row["Status"]),
                        CCSStoreID = Converter.TryToString(row["CCSStoreID"]),
                        SFStoreID = Converter.TryToString(row["SFStoreID"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        CreateBy = Converter.TryToString(row["ChineseName"])
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT count(*) FROM mt_cabinet c {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }

        internal static bool UpdateCabinet(Cabinet data)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                 "select count(*) from mt_cabinet where IsDeleted = 0 AND (Number=@Number OR DeviceCode=@DeviceCode) AND ID<>@ID;",
                             new MySqlParameter("@Number", data.Number),
                             new MySqlParameter("@DeviceCode", data.DeviceCode),
                             new MySqlParameter("@ID", data.ID));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("柜子编号或设备号已存在！");
            }

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"Update mt_cabinet set 
                        Name=@Name
                        ,Number=@Number
                        ,Province=@Province
                        ,City=@City
                        ,Country=@Country
                        ,Address=@Address
                        ,Longitude=@Longitude
                        ,Latitude=@Latitude
                        ,DeviceCode=@DeviceCode
                        ,Status=@Status
                        ,CCSStoreID=@CCSStoreID
                        ,SFStoreID=@SFStoreID
                        ,ContactPerson=@ContactPerson
                        ,ContactPhone=@ContactPhone
                        ,UpdateBy=@UpdateBy
                     where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@Name", data.Name),
                new MySqlParameter("@Number", data.Number),
                new MySqlParameter("@Province", data.Province),
                new MySqlParameter("@City", data.City),
                new MySqlParameter("@Country", data.Country),
                new MySqlParameter("@Address", data.Address),
                new MySqlParameter("@Longitude", data.Longitude),
                new MySqlParameter("@Latitude", data.Latitude),
                new MySqlParameter("@DeviceCode", data.DeviceCode),
                new MySqlParameter("@Status", data.Status),
                new MySqlParameter("@CCSStoreID", data.CCSStoreID),
                new MySqlParameter("@SFStoreID", data.SFStoreID),
                new MySqlParameter("@ContactPerson", data.ContactPerson),
                new MySqlParameter("@ContactPhone", data.ContactPhone),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }

        internal static bool AddNewCabinet(Cabinet data)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                 "select count(*) from mt_cabinet where IsDeleted = 0 AND (Number=@Number OR DeviceCode=@DeviceCode);",
                             new MySqlParameter("@Number", data.Number),
                             new MySqlParameter("@DeviceCode", data.DeviceCode));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("设备编号已存在！");
            }

            // 生成二维码图片
            data.QRCode = QRCodeHelper.Generator(data.QRCodeContent, data.Number);
            data.CreateBy = data.UpdateBy;
            string cabinetId = Guid.NewGuid().ToString();
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO mt_cabinet (
                        `ID`
                        ,`Name`
                        ,`Number`
                        ,`Province`
                        ,`City`
                        ,`Country`
                        ,`Address`
                        ,`Longitude`
                        ,`Latitude`
                        ,`DeviceCode`
                        ,`QRCode`		
                        ,`DeviceStatus`
                        ,`DeviceType`
                        ,`Status`
                        ,`CCSStoreID`
                        ,`SFStoreID`
                        ,`ContactPerson`
                        ,`ContactPhone`
                        ,`CreateBy`
                    ) VALUES (
                        @ID
                        ,@Name
                        ,@Number
                        ,@Province
                        ,@City
                        ,@Country
                        ,@Address
                        ,@Longitude
                        ,@Latitude
                        ,@DeviceCode
                        ,@QRCode	
                        ,@DeviceStatus
                        ,@DeviceType
                        ,@Status
                        ,@CCSStoreID
                        ,@SFStoreID
                        ,@ContactPerson
                        ,@ContactPhone
                        ,@CreateBy);",
                new MySqlParameter("@ID", cabinetId),
                new MySqlParameter("@Name", data.Name),
                new MySqlParameter("@Number", data.Number),
                new MySqlParameter("@Province", data.Province),
                new MySqlParameter("@City", data.City),
                new MySqlParameter("@Country", data.Country),
                new MySqlParameter("@Address", data.Address),
                new MySqlParameter("@Longitude", data.Longitude),
                new MySqlParameter("@Latitude", data.Latitude),
                new MySqlParameter("@DeviceCode", data.DeviceCode),
                new MySqlParameter("@QRCode", data.QRCode),
                new MySqlParameter("@DeviceStatus", data.DeviceStatus),
                new MySqlParameter("@DeviceType", data.DeviceType),
                new MySqlParameter("@Status", data.Status),
                new MySqlParameter("@CCSStoreID", data.CCSStoreID),
                new MySqlParameter("@SFStoreID", data.SFStoreID),
                new MySqlParameter("@ContactPerson", data.ContactPerson),
                new MySqlParameter("@ContactPhone", data.ContactPhone),
                new MySqlParameter("@CreateBy", data.CreateBy));

            // 生成格子
            if (data.DeviceType == "box")
            {
                GridBLL.GenerateGrid("fangmai-12", cabinetId);
            }
            return true;
        }

        internal static bool ReGenerateQRCode(Cabinet data)
        {
            data.QRCode = QRCodeHelper.Generator(data.QRCodeContent, data.Number);
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"Update mt_cabinet set 
                        QRCode=@QRCode
                        ,UpdateBy=@UpdateBy
                     where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@QRCode", data.QRCode),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }

        internal static bool DeleteById(Cabinet data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Update mt_cabinet SET IsDeleted=1,DeletedBy=@DeletedBy where ID=@ID;",
                new MySqlParameter("@DeletedBy", data.DeletedBy),
                new MySqlParameter("@ID", data.ID));
            return true;
        }

        internal static Cabinet GetById(string id)
        {
            string sql = @" SELECT
	                            c.`ID`										
                                ,c.`Name`										
                                ,c.`Number`									
                                ,c.`Province`									
                                ,c.`City`										
                                ,c.`Country`									
                                ,c.`Address`									
                                ,c.`Longitude`									
                                ,c.`Latitude`									
                                ,c.`DeviceCode`	
                                ,c.`QRCode`								
                                ,c.`DeviceStatus`								
                                ,c.`Status`									
                                ,c.`ContactPerson`								
                                ,c.`ContactPhone`								
                                ,c.`CreateTime`			
                            FROM mt_cabinet c
                            WHERE c.ID=@ID
                                AND c.IsDeleted = 0 ";

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, new MySqlParameter("@ID", id));

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Cabinet()
                {
                    ID = Converter.TryToString(row["ID"]),
                    Province = Converter.TryToString(row["Province"]),
                    City = Converter.TryToString(row["City"]),
                    Country = Converter.TryToString(row["Country"]),
                    Number = Converter.TryToString(row["Number"]),
                    Address = Converter.TryToString(row["Address"]),
                    Latitude = Converter.TryToSingle(row["Latitude"]),
                    Longitude = Converter.TryToSingle(row["Longitude"]),
                    ContactPerson = Converter.TryToString(row["ContactPerson"]),
                    ContactPhone = Converter.TryToString(row["ContactPhone"]),
                    QRCode = Converter.TryToString(row["QRCode"]),
                    Status = Converter.TryToString(row["Status"]),
                    CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss")
                };
            }

            return null;
        }
    }
}
