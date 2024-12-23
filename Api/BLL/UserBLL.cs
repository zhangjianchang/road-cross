﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Api.Entity;
using Api.Utilities;

namespace Api.BLL
{
    public class UserBLL
    {
        public static UserInfo GetUserDetail(string userName, string password)
        {
            password = password.ToUpper().ToMD5();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT
                    e.UserName,
                    e.ChineseName,
                    e.WaitSet,
                    e.EMail,
                    e.Avatar,
                    r.RoleName,
                    r.RoleId
                FROM user_info e
                INNER JOIN user_role r ON e.RoleID = r.roleid 
                WHERE e.UserName = @UserName and e.Password = @Password LIMIT 1",
                new MySqlParameter("@UserName", userName),
                new MySqlParameter("@Password", password));

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new UserInfo
                {
                    UserName = Converter.TryToString(row["UserName"]),
                    Avatar = Converter.TryToString(row["Avatar"]),
                    ChineseName = Converter.TryToString(row["ChineseName"]),
                    EMail = Converter.TryToString(row["EMail"]),
                    RoleName = Converter.TryToString(row["RoleName"]),
                    RoleId = Converter.TryToInt32(row["RoleId"]),
                    WaitSet = Converter.TryToInt16(row["WaitSet"]),
                };
            }
            else
            {
                return null;
            }
        }

        public static UserInfo GetUserDetail(string userName)
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT
                    e.UserName,
                    e.ChineseName,
                    e.WaitSet,
                    e.EMail,
                    e.Avatar,
                    e.TelPhone,
                    r.RoleName,
                    r.RoleId
                FROM user_info e
                INNER JOIN user_role r ON e.RoleID = r.roleid 
                WHERE e.UserName = @UserName LIMIT 1",
                new MySqlParameter("@UserName", userName));

            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new UserInfo
                {
                    UserName = Converter.TryToString(row["UserName"]),
                    Avatar = Converter.TryToString(row["Avatar"]),
                    ChineseName = Converter.TryToString(row["ChineseName"]),
                    TelPhone = Converter.TryToString(row["TelPhone"]),
                    EMail = Converter.TryToString(row["EMail"]),
                    RoleName = Converter.TryToString(row["RoleName"]),
                    RoleId = Converter.TryToInt32(row["RoleId"]),
                    WaitSet = Converter.TryToInt16(row["WaitSet"]),
                };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        internal static bool ReSetPassword(LoginInfo user)
        {
            var userInfo = GetUserDetail(user.UserName, user.OriginalPassword);
            if (userInfo == null)
            {
                throw new Exception("原始密码错误");
            }
            string password = user.Password.ToUpper().ToMD5();
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $"Update user_info set Password=@Password where UserName=@UserName;",
                new MySqlParameter("@UserName", user.UserName),
                new MySqlParameter("@Password", password));
            return true;
        }

        public static List<object> GetUserList()
        {
            List<object> userList = new();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(
                Config.DBConnection,
                @"SELECT 
                    e.UserName,e.ChineseName,e.RoleId,
                    r.RoleName
                FROM user_info e
                INNER JOIN cf_role r ON e.RoleID = r.RoleID order by LastUpdate DESC");

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    userList.Add(new
                    {
                        UserName = Converter.TryToString(row["UserName"]),
                        ChineseName = Converter.TryToString(row["ChineseName"]),
                        RoleName = Converter.TryToString(row["RoleName"]),
                        RoleId = Converter.TryToInt32(row["RoleId"]),
                    });
                }
            }

            return userList;
        }

        internal static bool ResetPassword(string userName)
        {
            // 两次MD5加密
            string password = Config.DefaultPassword.ToMD5().ToMD5();
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $"Update user_info set Password='{password}', WaitSet = 1 where UserName=@UserName;",
                new MySqlParameter("@UserName", userName));
            return true;
        }


        internal static bool SetPassword(string userName, string password)
        {
            password = password.ToUpper().ToMD5();
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $"Update user_info set Password=@Password, WaitSet=0 where UserName=@UserName;",
                new MySqlParameter("@UserName", userName),
                new MySqlParameter("@Password", password));
            return true;
        }

        internal static bool SaveUser(string userName, string chineseName, int roleId)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Update user_info set ChineseName=@ChineseName,RoleID=@RoleID where UserName=@UserName;",
                new MySqlParameter("@UserName", userName),
                new MySqlParameter("@ChineseName", chineseName),
                new MySqlParameter("@RoleID", roleId));
            return true;
        }

        internal static bool DeleteUser(string userName)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Delete from user_info where UserName=@UserName;",
                new MySqlParameter("@UserName", userName));
            return true;
        }

        public static bool AddNewUser(string userName, string chineseName, int roleId)
        {
            object re = JabMySqlHelper.ExecuteScalar(Config.DBConnection,
                              "select count(*) from user_info where UserName=@UserName;",
                          new MySqlParameter("@UserName", userName));
            if (Converter.TryToInt32(re) > 0)
            {
                throw new MsgException("用户已存在");
            }
            string password = Config.DefaultPassword.ToMD5().ToMD5();

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $"INSERT INTO user_info (UserName,ChineseName,RoleID,Password) VALUES (@UserName,@ChineseName,@RoleID,'{password}');",
                new MySqlParameter("@UserName", userName),
                new MySqlParameter("@ChineseName", chineseName),
                new MySqlParameter("@RoleID", roleId));
            return true;
        }

        public static bool SubmitSuggestion(SuggestionRequest request)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $"INSERT INTO user_suggestion (UserName,Suggestion,Status,CreateDate) VALUES (@UserName,@Suggestion,@Status,now());",
                new MySqlParameter("@UserName", request.UserName),
                new MySqlParameter("@Suggestion", request.Suggestion),
                new MySqlParameter("@Status", "100"));
            return true;
        }

        internal static bool UpdateSuggestion(SuggestionRequest request)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    "Update user_suggestion set Answer=@Answer,Status=@Status,AnswerDate=now() where ID=@ID;",
                new MySqlParameter("@Answer", request.Answer),
                new MySqlParameter("@Status", request.Status),
                new MySqlParameter("@ID", request.ID));
            return true;
        }

        public static List<SuggestionRequest> GetSuggestionLIst(SuggestionRequest request)
        {
            string where = "";
            if (!string.IsNullOrEmpty(request.Status))
            {
                where = " where status = " + request.Status;
            }
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, string.Format("select us.*,ui.Avatar from user_suggestion us left join user_info ui on us.UserName=ui.UserName {0} order by us.CreateDate desc", where));
            List<SuggestionRequest> designs = new();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    SuggestionRequest design = new()
                    {
                        ID = Converter.TryToInt32(row["ID"]),
                        UserName = row["UserName"].ToString(),
                        Avatar = row["Avatar"].ToString(),
                        Suggestion = row["Suggestion"].ToString(),
                        Status = row["Status"].ToString(),
                        CreateDate = Converter.TryToDateTime(row["CreateDate"]).ToString("yyyy-MM-dd"),
                        Answer = row["Answer"].ToString(),
                        AnswerDate = Converter.TryToDateTime(row["AnswerDate"]).ToString("yyyy-MM-d"),
                    };
                    designs.Add(design);
                }
            }
            return designs;
        }
    }
}
