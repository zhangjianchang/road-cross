using Api.Entity;
using Api.Utilities;
using MySql.Data.MySqlClient;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;

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
            string code = request.Code ?? GenerateMD5Code(request.MemberName, date);
            request.Status = "100";//待激活状态

            if (request.Type == "1")
            {
                request.ValidDate = 31;
                request.UsageTimes = 60;
            }
            else if (request.Type == "2")
            {
                request.ValidDate = 366;
                request.UsageTimes = 799;
            }
            else if (request.Type == "3")
            {
                request.ValidDate = request.Duration * 31;
                request.UsageTimes = request.Duration * 31 * 2;
            }
            else if (request.Type == "4")
            {
                //企业会员直接激活
                request.Status = "200";
                request.RemainingDays = DateDiff(DateTime.Now, Convert.ToDateTime(request.ExpireDate));//剩余天数，过期日期-当前时间
            }
            else
            {
                request.ValidDate = 0;
                request.UsageTimes = 0;
            }
            request.RemainingTimes = request.UsageTimes;

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                @$"INSERT INTO `main`.`authorization_code` 
                                            (`Code`, `Type`, `Duration`, `MemberName`, `ValidDate`, `UsageTimes`, `UsedTimes`, `ExpireDate`, `ActiveDate`, `RemainingTimes`, `RemainingDays`, `Status`, `CreateDate`)
                                       VALUES
                                            (@Code, @Type, @Duration, @MemberName, @ValidDate, @UsageTimes, @UsedTimes, @ExpireDate, @ActiveDate, @RemainingTimes, @RemainingDays, @Status, @CreateDate)",
                            new MySqlParameter("@Code", code),
                            new MySqlParameter("@Type", request.Type),
                            new MySqlParameter("@Duration", request.Duration),
                            new MySqlParameter("@MemberName", request.MemberName),
                            new MySqlParameter("@ValidDate", request.ValidDate),
                            new MySqlParameter("@UsageTimes", request.UsageTimes),
                            new MySqlParameter("@UsedTimes", request.UsedTimes),
                            new MySqlParameter("@ExpireDate", request.ExpireDate),
                            new MySqlParameter("@ActiveDate", request.ActiveDate),
                            new MySqlParameter("@RemainingTimes", request.RemainingTimes),
                            new MySqlParameter("@RemainingDays", request.RemainingDays),
                            new MySqlParameter("@Status", request.Status),
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
            request.ExpireDate = DateTime.Now.AddDays(Converter.TryToDouble(codeInfo.ValidDate)).ToString("G");
            request.RemainingDays = DateDiff(DateTime.Now, Convert.ToDateTime(request.ExpireDate));//剩余天数，过期日期-当前时间

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                $"UPDATE `main`.`authorization_code` SET `ActiveDate` = now(), `Status` = @Status, `ExpireDate` = @ExpireDate, `RemainingDays` = @RemainingDays WHERE `Code` = @Code AND `MemberName`=@MemberName",
                            new MySqlParameter("@Status", "200"),
                            new MySqlParameter("@ExpireDate", request.ExpireDate),
                            new MySqlParameter("@RemainingDays", request.RemainingDays),
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
                                    $"UPDATE `main`.`authorization_code` SET `UsedTimes` = @UsedTimes, `RemainingTimes` = @RemainingTimes, `RemainingDays` = @RemainingDays,`Status` = @Status  WHERE `ID` = @ID",
                                new MySqlParameter("@UsedTimes", request.UsedTimes),
                                new MySqlParameter("@RemainingTimes", request.RemainingTimes),
                                new MySqlParameter("@RemainingDays", request.RemainingDays),
                                new MySqlParameter("@Status", request.Status),
                                new MySqlParameter("@ID", request.ID));
            }
            else
            {
                request.Status = request.RemainingDays < 0 ? "400" : "300";
                JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                    $"UPDATE `main`.`authorization_code` SET `Status` = @Status  WHERE `ID` = @ID",
                new MySqlParameter("@Status", request.Status),
                new MySqlParameter("@ID", request.ID));
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
                                $"SELECT * FROM `main`.`authorization_code` WHERE `MemberName`=@MemberName AND status<>'100' ORDER BY Type DESC",
                            new MySqlParameter("@MemberName", memberName));
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var RemainingDays = Convert.ToInt32(row["RemainingDays"]);
                    var RemainingTimes = Convert.ToInt32(row["RemainingTimes"]);
                    AuthotizationCode code = new()
                    {
                        ID = row["ID"].ToString(),
                        Type = row["Type"].ToString(),
                        Code = row["Code"].ToString(),
                        AccountName = row["AccountName"].ToString(),
                        MemberName = row["MemberName"].ToString(),
                        ValidDate = Convert.ToInt32(row["ValidDate"].ToString()),
                        ActiveDate = row["ActiveDate"].ToString(),
                        ExpireDate = row["ExpireDate"].ToString(),
                        UsageTimes = Convert.ToInt32(row["UsageTimes"].ToString()),
                        UsedTimes = Convert.ToInt32(row["UsedTimes"].ToString()),
                        RemainingDays = RemainingDays < 0 ? 0 : RemainingDays,
                        RemainingTimes = RemainingTimes < 0 ? 0 : RemainingTimes,
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

        /// <summary>
        /// 授权企业账户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool ActiveEnterpriseAccount(EnterpriseAccountRequest request)
        {
            //先判断是否存在该用户
            var userInfo = UserBLL.GetUserDetail(request.MemberName) ?? throw new Exception("您输入的用户不存在，请核对后重新输入");
            if (IsSubAccountExist(request.MemberName)) throw new Exception("当前账号已授权为企业账户，并且账户可用，无需重复设置");
            //生成授权码
            string date = DateTime.Now.ToString("G");
            request.Code = GenerateMD5Code(request.MemberName, date);

            if (request.Type == "A")
            {
                request.Frequency = 6000;
            }
            else if (request.Type == "B")
            {
                request.Frequency = 12000;
            }
            request.ValidDate = 366;
            request.ExpireDate = DateTime.Now.AddDays(request.ValidDate).ToString("G");

            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                @$"INSERT INTO `main`.`enterprise_account` 
                                            (`Code`, `Type`, `MemberName`, `Frequency`, `ValidDate`, `ExpireDate`, `CreateDate`)
                                       VALUES
                                             (@Code, @Type, @MemberName, @Frequency, @ValidDate, @ExpireDate, now())",
                            new MySqlParameter("@Code", request.Code),
                            new MySqlParameter("@Type", request.Type),
                            new MySqlParameter("@MemberName", request.MemberName),
                            new MySqlParameter("@Frequency", request.Frequency),
                            new MySqlParameter("@ValidDate", request.ValidDate),
                            new MySqlParameter("@ExpireDate", request.ExpireDate));

            //激活当前账户
            AuthotizationCodeRequest request2 = new()
            {
                Type = "4",
                Code = request.Code,
                MemberName = request.MemberName,
                AccountName = "管理员",
                ValidDate = request.ValidDate,
                ActiveDate = DateTime.Now.ToString("G"),
                ExpireDate = request.ExpireDate,
                UsageTimes = request.Frequency,
            };
            GenerateCode(request2);
            //修改用户类型为企业用户
            UserBLL.SaveUser(userInfo.UserName, userInfo.ChineseName, 5);
            return true;
        }

        /// <summary>
        /// 查找所有子账户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static List<AuthotizationCode> GetSubAccountList(string userName)
        {
            string sql = @"SELECT
	                                    ea.Frequency,
	                                    ea.ExpireDate 'AdminExpireDate',
	                                    ea.ValidDate 'AdminValidDate',
	                                    ac.* 
                                    FROM
	                                    enterprise_account ea
	                                    LEFT JOIN authorization_code ac ON ea.`Code` = ac.`Code` 
                                    WHERE
	                                    ea.MemberName = @MemberName";
            List<AuthotizationCode> codes = new();
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, new MySqlParameter("@MemberName", userName));
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    AuthotizationCode code = new()
                    {
                        Frequency = Convert.ToInt32(row["Frequency"].ToString()),
                        AdminExpireDate = Converter.TryToDateTime(row["AdminExpireDate"]).ToString("yyyy-MM-dd HH:mm:ss"),
                        AdminValidDate = Convert.ToInt32(row["AdminValidDate"].ToString()),
                        ID = row["ID"].ToString(),
                        Type = row["Type"].ToString(),
                        Code = row["Code"].ToString(),
                        MemberName = row["MemberName"].ToString(),
                        AccountName = row["AccountName"].ToString(),
                        ValidDate = Convert.ToInt32(row["ValidDate"].ToString()),
                        ActiveDate = Converter.TryToDateTime(row["ActiveDate"]).ToString("yyyy-MM-dd HH:mm:ss"),
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
        /// 查找授权账号是否存在且是否过期
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsSubAccountExist(string userName)
        {
            //先查是否过期
            string sql = @"SELECT ExpireDate FROM enterprise_account WHERE MemberName = @MemberName";
            DataTable dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, new MySqlParameter("@MemberName", userName));
            bool isExist = false;
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string ExpireDate = row["ExpireDate"].ToString();
                    int RemainingDays = DateDiff(DateTime.Now, Convert.ToDateTime(ExpireDate));//剩余天数，过期日期-当前时间
                    isExist = RemainingDays > 0;//可用天数大于0，账户有效
                }
            }
            if (isExist) return true;
            //再查是否可用次数已使用完毕
            sql = @"SELECT
	                          ea.Frequency - SUM( ac.UsedTimes ) AS RemainingTimes 
                          FROM
	                          enterprise_account ea
	                          LEFT JOIN authorization_code ac ON ea.`Code` = ac.`Code` 
                          WHERE
	                          ea.MemberName = @MemberName";
            dt = JabMySqlHelper.ExecuteDataTable(Config.DBConnection, sql, new MySqlParameter("@MemberName", userName));
            return Converter.TryToInt32(dt.Rows[0]["RemainingTimes"]) > 0;//可用次数大于0，账户有效
        }

        /// <summary>
        /// 配置子账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool SetSubAccount(SubAccountRequest request)
        {
            foreach (var item in request.AccountList)
            {
                if (string.IsNullOrEmpty(item.ID))
                {
                    item.Code = request.Code;
                    item.Status = "200";
                    item.Type = "4";
                    item.Duration = 0;
                    item.RemainingDays = DateDiff(DateTime.Now, Convert.ToDateTime(item.ExpireDate));//剩余天数，过期日期-当前时间
                    AddSubAccount(item);
                }
                else
                {
                    UpdateAuthotizationCode(item);
                }
            }
            return true;
        }

        /// <summary>
        /// 添加企业账户子账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool AddSubAccount(AuthotizationCode request)
        {
            JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                              @$"INSERT INTO `main`.`authorization_code`
                                            (`Code`, `Type`, `Duration`, `MemberName`, `AccountName`,  `ValidDate`, `UsageTimes`, `UsedTimes`, `ExpireDate`, `ActiveDate`, `RemainingTimes`, `RemainingDays`, `Status`, `CreateDate`)
                                       VALUES
                                            (@Code, @Type, @Duration, @MemberName, @AccountName, @ValidDate, @UsageTimes, @UsedTimes, @ExpireDate, @ActiveDate, @RemainingTimes, @RemainingDays, @Status, now())",
                          new MySqlParameter("@Code", request.Code),
                          new MySqlParameter("@Type", request.Type),
                          new MySqlParameter("@Duration", request.Duration),
                          new MySqlParameter("@MemberName", request.MemberName),
                          new MySqlParameter("@AccountName", request.AccountName),
                          new MySqlParameter("@ValidDate", request.ValidDate),
                          new MySqlParameter("@UsageTimes", request.UsageTimes),
                          new MySqlParameter("@UsedTimes", request.UsedTimes),
                          new MySqlParameter("@ExpireDate", request.ExpireDate),
                          new MySqlParameter("@ActiveDate", request.ActiveDate),
                          new MySqlParameter("@RemainingTimes", request.RemainingTimes),
                          new MySqlParameter("@RemainingDays", request.RemainingDays),
                          new MySqlParameter("@Status", request.Status));
            return true;
        }

        /// <summary>
        /// 修改账户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool UpdateAuthotizationCode(AuthotizationCode request)
        {
            request.RemainingDays = DateDiff(DateTime.Now, Convert.ToDateTime(request.ExpireDate));//剩余天数，过期日期-当前时间
            int res = JabMySqlHelper.ExecuteNonQuery(Config.DBConnection,
                                $"UPDATE `main`.`authorization_code` SET `AccountName` = @AccountName, `UsageTimes` = @UsageTimes, `RemainingTimes` = @RemainingTimes, `RemainingDays`=@RemainingDays  WHERE `ID` = @ID",
                            new MySqlParameter("@AccountName", request.AccountName),
                            new MySqlParameter("@UsageTimes", request.UsageTimes),
                            new MySqlParameter("@RemainingTimes", request.RemainingTimes),
                            new MySqlParameter("@RemainingDays", request.RemainingDays),
                            new MySqlParameter("@ID", request.ID));
            return res > 0;
        }

        /// <summary>
        /// 删除企业账户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool DeleteAuthotizationCode(SubAccountRequest request)
        {
            int res = JabMySqlHelper.ExecuteNonQuery(Config.DBConnection, $"DELETE FROM `main`.`authorization_code`  WHERE `ID` = @ID", new MySqlParameter("@ID", request.ID));
            return res > 0;
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
