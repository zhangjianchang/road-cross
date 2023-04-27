using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Api.BLL;
using Api.Entity;
using Api.Utilities;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/User
        [HttpGet]
        public MyResult GetList()
        {
            List<Object> roleList = UserBLL.GetUserList();
            return MyResult.OK(roleList);
        }

        // POST: api/User
        [HttpPost]
        public MyResult Post([FromBody] object data)
        {
            JObject obj = JObject.FromObject(data);
            string userName = Convert.ToString(obj["userName"]);
            string chineseName = Convert.ToString(obj["chineseName"]);
            int roleId = Convert.ToInt32(obj["roleId"]);
            bool isNew = Convert.ToBoolean(obj["isNew"]);

            if (string.IsNullOrWhiteSpace(userName))
            {
                return MyResult.Error("用户名不能为空！");
            }
            if (string.IsNullOrWhiteSpace(chineseName))
            {
                return MyResult.Error("姓名不能为空！");
            }

            bool re = isNew ? UserBLL.AddNewUser(userName, chineseName, roleId) : UserBLL.SaveUser(userName, chineseName, roleId);
            return re ? MyResult.OK() : MyResult.Error();
        }

        [HttpPost("resetPassword")]
        public MyResult ResetPassword([FromBody] object data)
        {
            JObject obj = JObject.FromObject(data);
            string userName = Convert.ToString(obj["userName"]);

            if (string.IsNullOrWhiteSpace(userName))
            {
                return MyResult.Error("用户名不能为空！");
            }

            bool re = UserBLL.ResetPassword(userName);
            return re ? MyResult.OK() : MyResult.Error();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{userName}")]
        public MyResult Delete(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return MyResult.Error("用户名不能为空！");
            }

            bool re = UserBLL.DeleteUser(userName);
            return re ? MyResult.OK() : MyResult.Error();
        }

        [HttpPost("suggestion")]
        public MyResult SubmitSuggestion([FromBody] SuggestionRequest request)
        {
            try
            {
                request.UserName = Request.Headers["userName"];
                var res = UserBLL.SubmitSuggestion(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        [HttpPost("getSuggestionList")]
        public MyResult GetSuggestionList([FromBody] SuggestionRequest request)
        {
            try
            {
                var res = UserBLL.GetSuggestionLIst(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        [HttpPost("answer")]
        public MyResult AnswerSuggestion([FromBody] SuggestionRequest request)
        {
            try
            {
                var res = UserBLL.UpdateSuggestion(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
    }
}
