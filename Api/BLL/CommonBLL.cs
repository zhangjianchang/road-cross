using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Api.BLL
{
    public class CommonBLL
    {
        public static List<FilterOptions> GetContactPersonOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT DISTINCT ContactPerson
                    FROM mt_cabinet
                  order by ContactPerson ASC");

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    options.Add(new FilterOptions
                    {
                        Value = Converter.TryToString(row["ContactPerson"]),
                        Label = Converter.TryToString(row["ContactPerson"]),
                    });
                }
            }
            return options;
        }

        public static List<FilterOptions> GetCabinetNumOptions(string deviceType)
        {
            List<FilterOptions> options = new List<FilterOptions>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT DISTINCT Name,Number
                    FROM mt_cabinet
                    WHERE IsDeleted = 0 AND DeviceType=@DeviceType
                    ORDER BY Number ASC", new MySqlParameter("@DeviceType", deviceType));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    options.Add(new FilterOptions
                    {
                        Value = Converter.TryToString(row["Number"]),
                        Label = Converter.TryToString(row["Name"]),
                    });
                }
            }
            return options;
        }


        public static List<FilterOptions> GetCabinetNumWithIDOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT ID, Name
                    FROM mt_cabinet
                    WHERE IsDeleted = 0
                    ORDER BY Number ASC");

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    options.Add(new FilterOptions
                    {
                        Value = Converter.TryToString(row["ID"]),
                        Label = Converter.TryToString(row["Name"]),
                    });
                }
            }
            return options;
        }

        internal static List<FilterOptions> GetPartnerOrganizationOptions(string type)
        {
            List<FilterOptions> options = new List<FilterOptions>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT distinct Organization
                    FROM mt_partner
					WHERE Organization is not null and Organization<>''
                            AND `Type` = @Type
                    ORDER BY Organization ASC;", new MySqlParameter("@Type", type));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    options.Add(new FilterOptions
                    {
                        Value = Converter.TryToString(row["Organization"]),
                        Label = Converter.TryToString(row["Organization"]),
                    });
                }
            }
            return options;
        }

        internal static List<FilterOptions> GetBottleSpecOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT distinct Name,ID
                    FROM mt_bottletype
                    WHERE IsDeleted = 0
                    ORDER BY Name ASC;");

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    options.Add(new FilterOptions
                    {
                        Value = Converter.TryToString(row["ID"]),
                        Label = Converter.TryToString(row["Name"]),
                    });
                }
            }
            return options;
        }



        internal static List<FilterOptions> GetCouponsOptions()
        {
            List<FilterOptions> options = new List<FilterOptions>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT distinct Name,ID
                    FROM mt_coupons
                    ORDER BY Name ASC;");

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    options.Add(new FilterOptions
                    {
                        Value = Converter.TryToString(row["ID"]),
                        Label = Converter.TryToString(row["Name"]),
                    });
                }
            }
            return options;
        }
    }
}
