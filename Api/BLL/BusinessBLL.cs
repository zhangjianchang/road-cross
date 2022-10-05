using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Entity;
using Api.Utilities;

namespace Api.BLL
{
    public class BusinessBLL
    {
        public static object GetAgreements(string code)
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT
                    c.Content
                    ,c.UpdateTime
                    ,e.ChineseName as UpdateBy
                FROM mt_agreements c
                LEFT JOIN mt_employee e ON c.UpdateBy = e.UserName 
                WHERE Code=@Code;",
                new MySqlParameter("@Code", code));

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new
                {
                    Content = Converter.TryToString(row["Content"]),
                    UpdateTime = Converter.TryToDateTime(row["UpdateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdateBy = Converter.TryToString(row["UpdateBy"]),
                };
            }
            else
            {
                return null;
            }
        }

        internal static bool SaveAgreements(string code, string content, string updateBy)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO `mt_agreements` (`Code`,`Content`,`UpdateBy`)
                      VALUES (@Code,@Content,@UpdateBy)
                      ON DUPLICATE KEY UPDATE `Content`=@Content,`UpdateBy`=@UpdateBy,`UpdateTime` = NOW();",
                new MySqlParameter("@Code", code),
                new MySqlParameter("@Content", content),
                new MySqlParameter("@UpdateBy", updateBy));
            return true;
        }

        #region 常见问题分类
        internal static List<Article> GetArticleCategoryList()
        {
            List<Article> list = new List<Article>();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT 
                    ID,
                    Name,
                    DisplayOrder,
                    ShowFlag
                FROM mt_article_category
                ORDER BY Name");

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new Article()
                    {
                        ID = Converter.TryToInt32(row["ID"]),
                        Name = Converter.TryToString(row["Name"]),
                        DisplayOrder = Converter.TryToInt32(row["DisplayOrder"]),
                        ShowFlag = Converter.TryToInt32(row["ShowFlag"]),
                    });
                }
            }

            return list;
        }

        internal static bool SaveArticleCategory(Article data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"UPDATE `mt_article_category`
                        SET `ID` = @ID,
                            `Name` = @Name,
                            `DisplayOrder` = @DisplayOrder,
                            `ShowFlag` = @ShowFlag,
                            `UpdateBy` = @UpdateBy
                        WHERE `ID` = @ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@Name", data.Name),
                new MySqlParameter("@DisplayOrder", data.DisplayOrder),
                new MySqlParameter("@ShowFlag", data.ShowFlag),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }

        internal static bool DeleteArticleCategory(int id)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Delete from `mt_article_category` where `ID` = @ID;",
                new MySqlParameter("@ID", id));
            return true;
        }

        public static bool AddArticleCategory(Article data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO `mt_article_category`
                        (`Name`,
                        `DisplayOrder`,
                        `ShowFlag`,
                        `CreateBy`,
                        `UpdateTime`)
                      VALUES
                        (@Name,
                        @DisplayOrder,
                        @ShowFlag,
                        @CreateBy,
                        NOW());",
                new MySqlParameter("@Name", data.Name),
                new MySqlParameter("@DisplayOrder", data.DisplayOrder),
                new MySqlParameter("@ShowFlag", data.ShowFlag),
                new MySqlParameter("@CreateBy", data.UpdateBy));
            return true;
        }

        #endregion


        #region 常见问题
        internal static List<Article> GetArticleList()
        {
            List<Article> recordList = new List<Article>();
            string sql = @" SELECT a.`ID`,
                                a.`CategoryID`,
                                a.`Title`,
                                a.`Content`,
                                a.`DisplayOrder`,
                                a.`ShowFlag`,
                                a.`CreateTime`,
                                a.`UpdateTime`,
                                b.`Name` AS `CategoryName`
                            FROM `mt_article` a
                            INNER JOIN `mt_article_category` b ON a.`CategoryID` = b.`ID`
                            ORDER BY
	                            a.`UpdateTime` DESC ";

            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    recordList.Add(new Article()
                    {
                        ID = Converter.TryToInt32(row["ID"]),
                        DisplayOrder = Converter.TryToInt32(row["DisplayOrder"]),
                        ShowFlag = Converter.TryToInt32(row["ShowFlag"]),
                        CategoryID = Converter.TryToInt32(row["CategoryID"]),
                        CategoryName = Converter.TryToString(row["CategoryName"]),
                        Title = Converter.TryToString(row["Title"]),
                        Content = Converter.TryToString(row["Content"]),
                        CreateTime = Converter.TryToDateTime(row["CreateTime"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateTime = Converter.TryToDateTime(row["UpdateTime"]).ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }
            return recordList;
        }

        internal static bool SaveArticle(Article data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"UPDATE `mt_article`
                        SET `ID` = @ID,
                            `CategoryID` = @CategoryID,
                            `Title` = @Title,
                            `Content` = @Content,
                            `DisplayOrder` = @DisplayOrder,
                            `ShowFlag` = @ShowFlag,
                            `UpdateBy` = @UpdateBy
                        WHERE `ID` = @ID;",
                new MySqlParameter("@ID", data.ID),
                new MySqlParameter("@CategoryID", data.CategoryID),
                new MySqlParameter("@Title", data.Title),
                new MySqlParameter("@Content", data.Content),
                new MySqlParameter("@DisplayOrder", data.DisplayOrder),
                new MySqlParameter("@ShowFlag", data.ShowFlag),
                new MySqlParameter("@UpdateBy", data.UpdateBy));
            return true;
        }

        internal static bool DeleteArticle(int id)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Delete from `mt_article` where `ID` = @ID;",
                new MySqlParameter("@ID", id));
            return true;
        }

        public static bool AddArticle(Article data)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    @"INSERT INTO `mt_article`
                        (`CategoryID`,
                        `Title`,
                        `Content`,
                        `DisplayOrder`,
                        `ShowFlag`,
                        `CreateBy`,
                        `UpdateTime`)
                      VALUES
                        (@CategoryID,
                        @Title,
                        @Content,
                        @DisplayOrder,
                        @ShowFlag,
                        @CreateBy,
                        NOW());",
                new MySqlParameter("@CategoryID", data.CategoryID),
                new MySqlParameter("@Title", data.Title),
                new MySqlParameter("@Content", data.Content),
                new MySqlParameter("@DisplayOrder", data.DisplayOrder),
                new MySqlParameter("@ShowFlag", data.ShowFlag),
                new MySqlParameter("@CreateBy", data.UpdateBy));
            return true;
        }

        #endregion

    }
}
