using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Api.BLL
{
    public class GridBLL
    {
        internal static List<Grid> GetGridList(GridParam searchParam, out int total)
        {
            int offset = (searchParam.PageIndex - 1) * searchParam.PageSize;
            int rows = searchParam.PageSize;

            List<Grid> recordList = new List<Grid>();
            string sql = @" SELECT
	                            g.`ID`,
	                            g.`CabinetID`,
	                            c.`Number` as CabinetNumber,
	                            c.`Name` as CabinetName,
	                            g.`GridDeviceCode`,
	                            g.`GridNumber`,
	                            g.`Position`,
	                            g.`Status`,
	                            g.`DoorStatus`,
	                            g.`CreateTime`,
	                            e.`ChineseName`,
	                            b.`Code` AS BottleCode,
	                            t.`Name` AS BottleSpec
                            FROM
	                            mt_grid g
	                            INNER JOIN mt_cabinet c ON g.CabinetID = c.ID
	                            LEFT JOIN mt_employee e ON c.CreateBy = e.UserName
	                            LEFT JOIN mt_grid_bottle gb ON gb.GridID = g.ID
	                            LEFT JOIN mt_bottle b ON b.ID = gb.BottleID
	                            LEFT JOIN mt_bottletype t ON t.ID= b.Spec
                            {0}
                            ORDER by c.`Number`,CAST(g.`GridNumber` AS signed)
                            LIMIT {1},{2} ";

            string where = " WHERE c.IsDeleted = 0 ";
            List<MySqlParameter> param = new List<MySqlParameter>();

            if (!string.IsNullOrEmpty(searchParam.CabinetID))
            {
                where += " AND g.`CabinetID` = @CabinetID";
                param.Add(new MySqlParameter("@CabinetID", searchParam.CabinetID));
            }
            if (!string.IsNullOrEmpty(searchParam.GridNumber))
            {
                where += " AND g.`GridNumber` = @GridNumber";
                param.Add(new MySqlParameter("@GridNumber", searchParam.GridNumber));
            }
            if (!string.IsNullOrEmpty(searchParam.Status))
            {
                where += " AND g.`Status` = @Status";
                param.Add(new MySqlParameter("@Status", searchParam.Status));
            }

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format(sql, where, offset, rows), param.ToArray());

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Grid()
                    {
                        ID = Converter.TryToString(row["ID"]),
                        CabinetID = Converter.TryToString(row["CabinetID"]),
                        CabinetNumber = Converter.TryToString(row["CabinetNumber"]),
                        CabinetName = Converter.TryToString(row["CabinetName"]),
                        GridDeviceCode = Converter.TryToString(row["GridDeviceCode"]),
                        GridNumber = Converter.TryToString(row["GridNumber"]),
                        Position = Converter.TryToString(row["Position"]),
                        Status = Converter.TryToString(row["Status"]),
                        DoorStatus = Converter.TryToString(row["DoorStatus"]),
                        BottleCode = Converter.TryToString(row["BottleCode"]),
                        BottleSpec = Converter.TryToString(row["BottleSpec"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        CreateBy = Converter.TryToString(row["ChineseName"])
                    });
                }
            }

            // 查询总数
            string sqlCount = @"SELECT COUNT(*) 
                                FROM
	                                mt_grid g
	                                INNER JOIN mt_cabinet c ON g.CabinetID = c.ID
                                {0}";
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection, string.Format(sqlCount, where), param.ToArray());
            total = Convert.ToInt32(re);

            return recordList;
        }
        internal static bool GenerateGrid(string templeteType, string cabinetID)
        {
            object re = JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                 @"INSERT INTO `mt_grid`
                                        (`ID`,`CabinetID`,`GridNumber`,`Status`,`DoorStatus`,`CreateBy`)
                                    SELECT UUID(), @CabinetID, GridNumber, 1, 1, 'system'
                                    FROM app_grid_templete
                                    WHERE TempleteType = @TempleteType;",
                             new MySqlParameter("@TempleteType", templeteType),
                             new MySqlParameter("@CabinetID", cabinetID));
            return true;
        }


        internal static bool UpdateGrid(Grid data)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                 "select count(*) from mt_grid where CabinetID=@CabinetID AND GridNumber=@GridNumber AND ID<>@ID;",
                             new MySqlParameter("@GridNumber", data.GridNumber),
                             new MySqlParameter("@CabinetID", data.CabinetID),
                             new MySqlParameter("@ID", data.ID));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("格子编号已存在！");
            }

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"Update mt_grid set 
                        GridNumber=@GridNumber
                        ,Position=@Position
                        ,Status=@Status
                        ,DoorStatus=@DoorStatus
                        ,UpdateBy=@UpdateBy
                     where ID=@ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@GridNumber", data.GridNumber),
                new MySqlParameter("@Position", data.Position),
                new MySqlParameter("@Status", data.Status),
                new MySqlParameter("@DoorStatus", data.DoorStatus),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }

        internal static bool AddNewGrid(Grid data)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                 "select count(*) from mt_grid where CabinetID=@CabinetID AND GridNumber=@GridNumber;",
                             new MySqlParameter("@GridNumber", data.GridNumber),
                             new MySqlParameter("@CabinetID", data.CabinetID));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("格子编号已存在！");
            }

            data.CreateBy = data.UpdateBy;
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO mt_grid (
                        `ID`
                        ,`CabinetID`
                        ,`GridDeviceCode`
                        ,`GridNumber`
                        ,`Position`
                        ,`Status`
                        ,`DoorStatus`
                        ,`CreateBy`
                    ) VALUES (
                        @ID
                        ,@CabinetID
                        ,@GridDeviceCode
                        ,@GridNumber
                        ,@Position
                        ,@Status
                        ,@DoorStatus
                        ,@CreateBy);",
                new MySqlParameter("@ID", Guid.NewGuid().ToString()),
                new MySqlParameter("@CabinetID", data.CabinetID),
                new MySqlParameter("@GridDeviceCode", data.GridDeviceCode),
                new MySqlParameter("@GridNumber", data.GridNumber),
                new MySqlParameter("@Position", data.Position),
                new MySqlParameter("@Status", data.Status),
                new MySqlParameter("@DoorStatus", data.DoorStatus),
                new MySqlParameter("@CreateBy", data.CreateBy));
            return true;
        }

        internal static bool DeleteById(Grid data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "DELETE FROM mt_grid where ID=@ID;DELETE FROM mt_grid_bottle where GridID=@ID;",
                new MySqlParameter("@ID", data.ID));
            return true;
        }

        internal static bool Bind(Grid data)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                                    "select count(*) from mt_grid_bottle where GridID=@GridID OR BottleID=@BottleID;",
                                new MySqlParameter("@GridID", data.GridID),
                                new MySqlParameter("@BottleID", data.BottleID));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("绑定失败，格子或瓶子已被占用！");
            }

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO mt_grid_bottle (
                        `GridID`
                        ,`BottleID`
                        ,`CreateBy`
                    ) VALUES (
                        @GridID
                        ,@BottleID
                        ,@CreateBy);",
                new MySqlParameter("@GridID", data.GridID),
                new MySqlParameter("@BottleID", data.BottleID),
                new MySqlParameter("@CreateBy", data.CreateBy));
            return true;
        }

        internal static bool Unbind(Grid data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "DELETE FROM mt_grid_bottle where GridID=@GridID;",
                new MySqlParameter("@GridID", data.GridID));
            return true;
        }
    }
}
