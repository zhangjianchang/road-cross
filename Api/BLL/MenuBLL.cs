using System.Collections.Generic;
using System.Data;
using System.Linq;
using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;

namespace Api.BLL
{
    public static class MenuBLL
    {
        public static List<MenuEntity> GetMenuList()
        {
            List<MenuEntity> menuList = new List<MenuEntity>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, "select * from cf_menus where ShowFlag=1 order by DisplayOrder ", null);

            if (dt != null && dt.Rows.Count > 0)
            {
                List<MenuEntity> tempList = new List<MenuEntity>();
                foreach (DataRow row in dt.Rows)
                {
                    tempList.Add(new MenuEntity()
                    {
                        MenuID = Converter.TryToInt32(row["MenuID"]),
                        MenuText = Converter.TryToString(row["MenuText"]),
                        Icon = Converter.TryToString(row["Icon"]),
                        RouterLink = Converter.TryToString(row["RouterLink"]),
                        ParentID = Converter.TryToInt32(row["ParentID"]),
                    }); ;
                }
                menuList = tempList.Where(m => m.ParentID == -1).ToList();
                foreach (MenuEntity item in menuList)
                {
                    item.Children = tempList.Where(m => m.ParentID == item.MenuID).ToList();
                }
            }

            return menuList;
        }

        public static List<MenuEntity> GetAllMenuList()
        {
            List<MenuEntity> menuList = new List<MenuEntity>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, "select * from cf_menus order by ParentID, DisplayOrder ", null);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    menuList.Add(new MenuEntity()
                    {
                        MenuID = Converter.TryToInt32(row["MenuID"]),
                        MenuText = Converter.TryToString(row["MenuText"]),
                        Icon = Converter.TryToString(row["Icon"]),
                        RouterLink = Converter.TryToString(row["RouterLink"]),
                        ParentID = Converter.TryToInt32(row["ParentID"]),
                        ShowFlag = Converter.TryToInt32(row["ShowFlag"]),
                        DisplayOrder = Converter.TryToInt32(row["DisplayOrder"]),
                    }); ;
                }
            }

            return menuList;
        }

        internal static bool SaveMenu(MenuEntity data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"UPDATE `cf_menus`
                      SET
                        `MenuText` = @MenuText,
                        `Icon` = @Icon,
                        `RouterLink` = @RouterLink,
                        `DisplayOrder` = @DisplayOrder,
                        `ParentID` = @ParentID,
                        `ShowFlag` = @ShowFlag
                        WHERE `MenuID` = @MenuID;",
                new MySqlParameter("@MenuID", data.MenuID),
                new MySqlParameter("@MenuText", data.MenuText),
                new MySqlParameter("@Icon", data.Icon),
                new MySqlParameter("@RouterLink", data.RouterLink),
                new MySqlParameter("@DisplayOrder", data.DisplayOrder),
                new MySqlParameter("@ParentID", data.ParentID),
                new MySqlParameter("@ShowFlag", data.ShowFlag));
            return true;
        }

        internal static bool DeleteMenu(int menuID)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Delete from cf_menus where MenuID=@MenuID;",
                new MySqlParameter("@MenuID", menuID));
            return true;
        }

        public static bool AddMenu(MenuEntity data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO `cf_menus`
                        (`MenuText`,
                        `Icon`,
                        `RouterLink`,
                        `DisplayOrder`,
                        `ParentID`,
                        `ShowFlag`)
                      VALUES
                        (@MenuText,
                        @Icon,
                        @RouterLink,
                        @DisplayOrder,
                        @ParentID,
                        @ShowFlag);",
                new MySqlParameter("@MenuText", data.MenuText),
                new MySqlParameter("@Icon", data.Icon),
                new MySqlParameter("@RouterLink", data.RouterLink),
                new MySqlParameter("@DisplayOrder", data.DisplayOrder),
                new MySqlParameter("@ParentID", data.ParentID),
                new MySqlParameter("@ShowFlag", data.ShowFlag));
            return true;
        }
    }
}
