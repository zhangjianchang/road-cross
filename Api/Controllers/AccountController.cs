﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Api.BLL;
using Api.Entity;
using Api.Utilities;
using System;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // POST: api/Login
        [HttpPost("login")]
        public MyResult Post(LoginInfo user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                return MyResult.Error("用户名不能为空！");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return MyResult.Error("密码不能为空！");
            }

            // 读取用户信息
            UserInfo userInfo = UserBLL.GetUserDetail(user.UserName, user.Password);

            if (userInfo != null)
            {
                // 缓存一天
                userInfo.Token = JObject.FromObject(user).ToString().ToMD5();
                CacheHelper.CacheInsertAddMinutes(userInfo.Token, user, 24 * 60);
                return MyResult.OK(userInfo);
            }
            else
            {
                throw new MsgException("用户名或密码错误！");
            }
        }

        [HttpPost("setPassword")]
        public MyResult SetPassword([FromBody] LoginInfo user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                return MyResult.Error("用户名不能为空！");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return MyResult.Error("密码不能为空！");
            }
            UserBLL.SetPassword(user.UserName, user.Password);
            return MyResult.OK();
        }

        [HttpPost("reSetPwd")]
        public MyResult ReSetPassword(LoginInfo user)
        {
            try
            {
                user.UserName = Request.Headers["userName"];
                UserBLL.ReSetPassword(user);
                return MyResult.OK();
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        [HttpGet("logout")]
        public MyResult Get(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return MyResult.Error("参数错误！");
            }
            CacheHelper.CacheNull(token);
            return MyResult.OK();
        }

        [HttpGet("checkToken")]
        public MyResult CheckToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return MyResult.Error("参数错误！");
            }
            object value = CacheHelper.CacheValue(token);
            return value == null ? MyResult.Error() : MyResult.OK();
        }
    }
}
