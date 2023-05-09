using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;

namespace Api.BLL
{
    public class DesignBLL
    {
        /// <summary>
        /// 插入/更新
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string Save(SaveRequest request)
        {
            //插入
            if (string.IsNullOrEmpty(request.Guid))
            {
                string Guid = UUID.Generate();
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                     $"INSERT INTO `main`.`design` (`GUID`, `RoadName`, `DesignJson`, `CreateDate`, `CreateUser`, `UpdateDate`, `IsDeleted`) VALUES (@GUID, @RoadName, @DesignJson, now(),@UserName ,now(), 0)",
                                 new MySqlParameter("@GUID", Guid),
                                 new MySqlParameter("@UserName", request.UserName),
                                 new MySqlParameter("@RoadName", request.RoadName),
                                 new MySqlParameter("@DesignJson", request.DesignJson));
                return Guid;
            }
            //更新
            else
            {
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                    $"UPDATE `main`.`design` SET `RoadName` = @RoadName, `DesignJson` = @DesignJson, `UpdateDate` = now() WHERE `GUID` = @GUID",
                                new MySqlParameter("@GUID", request.Guid),
                                new MySqlParameter("@RoadName", request.RoadName),
                                new MySqlParameter("@DesignJson", request.DesignJson));
                return request.Guid;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool Delete(SaveRequest request)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                $"UPDATE `main`.`design` SET `IsDeleted` = 1, `UpdateDate` = now() WHERE `GUID` = @GUID",
                            new MySqlParameter("@GUID", request.Guid));
            return true;
        }

        /// <summary>
        /// 获取当前设计
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Design GetByGuid(SaveRequest request)
        {
            Design design = new Design();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, "select * from design where GUID =@GUID AND IsDeleted<>1", new MySqlParameter("@GUID", request.Guid));
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                design = new Design()
                {
                    Guid = row["Guid"].ToString(),
                    DesignJson = row["DesignJson"].ToString(),
                    RoadName = row["RoadName"].ToString(),
                };
            }
            return design;
        }

        /// <summary>
        /// 获取我全部的设计
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static List<Design> GetLIstByUser(string userName)
        {
            List<Design> designs = new List<Design>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, "select * from design where CreateUser =@UserName AND IsDeleted<>1  order by CreateDate desc", new MySqlParameter("@UserName", userName));
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Design design = new Design()
                    {
                        Guid = row["Guid"].ToString(),
                        //DesignJson = row["DesignJson"].ToString(),
                        RoadName = row["RoadName"].ToString(),
                        CreateDate = row["CreateDate"].ToString(),
                        UpdateDate = row["UpdateDate"].ToString(),
                    };
                    designs.Add(design);
                }
            }
            return designs;
        }
    }
}
