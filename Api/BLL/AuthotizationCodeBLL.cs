using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;

namespace Api.BLL
{
    public class AuthotizationCodeBLL
    {
        /// <summary>
        /// 生成授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GenerateCode(AuthotizationCodeRequest request)
        {
            //先判断是否存在该用户
            _ = UserBLL.GetUserDetail(request.MemberName) ?? throw new Exception("您输入的用户不存在，请核对后重新输入");

            string date = DateTime.Now.ToString("G");
            string code = GenerateMD5Code(request.MemberName, date);

            request.ValidDate = request.Type == "1" ? 31 : request.Type == "2" ? 366 : 0;
            request.RemainingDays = request.ValidDate;
            request.UsageTimes = request.Type == "1" ? 60 : request.Type == "2" ? 799 : 0;
            request.RemainingTimes = request.UsageTimes;

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                @$"INSERT INTO `main`.`authorization_code` 
                                            (`Code`, `Type`, `MemberName`, `ValidDate`, `UsageTimes`, `UsedTimes`, `RemainingTimes`, `RemainingDays`, `Status`, `CreateDate`)
                                       VALUES
                                            (@Code, @Type, @MemberName, @ValidDate, @UsageTimes, @UsedTimes, @RemainingTimes, @RemainingDays, @Status, @CreateDate)",
                            new MySqlParameter("@Code", code),
                            new MySqlParameter("@Type", request.Type),
                            new MySqlParameter("@MemberName", request.MemberName),
                            new MySqlParameter("@ValidDate", request.ValidDate),
                            new MySqlParameter("@UsageTimes", request.UsageTimes),
                            new MySqlParameter("@UsedTimes", request.UsedTimes),
                            new MySqlParameter("@RemainingTimes", request.RemainingTimes),
                            new MySqlParameter("@RemainingDays", request.RemainingDays),
                            new MySqlParameter("@Status", "100"),
                            new MySqlParameter("@CreateDate", date));
            return code;
        }
        /// <summary>
        /// 激活授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool ActiveCode(AuthotizationCodeRequest request)
        {
            //校验授权码是否正确
            request.Status = "100";
            var codeInfo = GetCodeInfo(request) ?? throw new Exception("授权码无效");

            string code = GenerateMD5Code(request.MemberName, codeInfo.CreateDate);
            if (code != codeInfo.Code) throw new Exception("授权码无效");
            request.ExpireDate = codeInfo.Type == "1" ? DateTime.Now.AddDays(31).ToString("G") : codeInfo.Type == "2" ? DateTime.Now.AddDays(366).ToString("G") : "";

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                $"UPDATE `main`.`authorization_code` SET `ActiveDate` = now(), `Status` = @Status, `ExpireDate` = @ExpireDate WHERE `Code` = @Code AND `MemberName`=@MemberName",
                            new MySqlParameter("@Status", "200"),
                            new MySqlParameter("@ExpireDate", request.ExpireDate),
                            new MySqlParameter("@Code", request.Code),
                            new MySqlParameter("@MemberName", request.MemberName));
            return true;
        }
        /// <summary>
        /// 使用/更新授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static AuthotizationCode UseCode(AuthotizationCodeRequest request)
        {
            //查找是否可用
            request.Status = "200";
            var code = GetCodeInfo(request) ?? throw new Exception("当前授权码无效");

            request.UsedTimes = code.UsedTimes + 1;//已使用次数+1
            request.RemainingTimes = code.RemainingTimes - 1;//剩余次数-1
            request.RemainingDays = DateDiff(DateTime.Now, Convert.ToDateTime(code.ExpireDate));//剩余天数，过期日期-当前时间

            if (request.RemainingDays >= 0 && request.RemainingTimes >= 0)
            {
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                    $"UPDATE `main`.`authorization_code` SET `UsedTimes` = @UsedTimes, `RemainingTimes` = @RemainingTimes, `RemainingDays` = @RemainingDays,`Status` = @Status  WHERE `Code` = @Code",
                                new MySqlParameter("@UsedTimes", request.UsedTimes),
                                new MySqlParameter("@RemainingTimes", request.RemainingTimes),
                                new MySqlParameter("@RemainingDays", request.RemainingDays),
                                new MySqlParameter("@Status", request.Status),
                                new MySqlParameter("@Code", request.Code));
            }
            else
            {
                request.Status = request.RemainingDays < 0 ? "400" : "300";
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $"UPDATE `main`.`authorization_code` SET `Status` = @Status  WHERE `Code` = @Code",
                new MySqlParameter("@Status", request.Status),
                new MySqlParameter("@Code", request.Code));
                throw new Exception("当前授权码已不可使用，请重新购买");
            }

            return GetCodeInfo(request);
        }

        /// <summary>
        /// 查找用户名下全部code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static List<AuthotizationCode> GetCodeInfosByUser(string memberName)
        {
            List<AuthotizationCode> codes = new();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection,
                                $"SELECT * FROM `main`.`authorization_code` WHERE `MemberName`=@MemberName AND status<>'100'",
                            new MySqlParameter("@MemberName", memberName));
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    AuthotizationCode code = new()
                    {
                        ID = row["ID"].ToString(),
                        Type = row["Type"].ToString(),
                        Code = row["Code"].ToString(),
                        MemberName = row["MemberName"].ToString(),
                        ValidDate = Convert.ToInt32(row["ValidDate"].ToString()),
                        ActiveDate = row["ActiveDate"].ToString(),
                        ExpireDate = row["ExpireDate"].ToString(),
                        UsageTimes = Convert.ToInt32(row["UsageTimes"].ToString()),
                        UsedTimes = Convert.ToInt32(row["UsedTimes"].ToString()),
                        RemainingDays = Convert.ToInt32(row["RemainingDays"].ToString()),
                        RemainingTimes = Convert.ToInt32(row["RemainingTimes"].ToString()),
                        Status = row["Status"].ToString(),
                    };
                    codes.Add(code);
                }
            }
            return codes;
        }

        /// <summary>
        /// 获取当前code信息（可用code）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static AuthotizationCode GetCodeInfo(AuthotizationCodeRequest request)
        {
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection,
                                      $"SELECT * FROM `main`.`authorization_code` WHERE `Code` = @Code AND `MemberName`=@MemberName AND `Status`=@Status",
                                  new MySqlParameter("@Status", request.Status),
                                  new MySqlParameter("@Code", request.Code),
                                  new MySqlParameter("@MemberName", request.MemberName));
            if (dt != null && dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                AuthotizationCode code = new()
                {
                    ID = row["ID"].ToString(),
                    Type = row["Type"].ToString(),
                    Code = row["Code"].ToString(),
                    MemberName = row["MemberName"].ToString(),
                    ValidDate = Convert.ToInt32(row["ValidDate"].ToString()),
                    ActiveDate = row["ActiveDate"].ToString(),
                    ExpireDate = row["ExpireDate"].ToString(),
                    UsageTimes = Convert.ToInt32(row["UsageTimes"].ToString()),
                    UsedTimes = Convert.ToInt32(row["UsedTimes"].ToString()),
                    RemainingDays = Convert.ToInt32(row["RemainingDays"].ToString()),
                    RemainingTimes = Convert.ToInt32(row["RemainingTimes"].ToString()),
                    Status = row["Status"].ToString(),
                    CreateDate = row["CreateDate"].ToString(),
                };
                return code;
            }
            return null;
        }


        #region 方法
        /// <summary>
        /// 两个日期之间相差的天数
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns></returns>
        private static int DateDiff(DateTime dateStart, DateTime dateEnd)
        {
            DateTime start = Convert.ToDateTime(dateStart.ToShortDateString());
            DateTime end = Convert.ToDateTime(dateEnd.ToShortDateString());

            TimeSpan sp = end.Subtract(start);
            return sp.Days;
        }
        /// <summary>
        /// 生成授权码
        /// </summary>
        /// <param name="memberName">用户名</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        private static string GenerateMD5Code(string memberName, string date)
        {
            string base_data = memberName + "!@#$%" + date;//编码基数，用户名!@#$%创建时间
            string code = base_data.ToMD5().ToMD5();//两次MD5加密
            return code;
        }
        #endregion
    }
}
