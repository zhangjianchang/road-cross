using Api.BLL;
using Api.Entity;
using Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthotizationCodeController : ControllerBase
    {
        /// <summary>
        /// 生成授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("generateCode")]
        public MyResult GenerateCode(AuthotizationCodeRequest request)
        {
            try
            {
                string code = AuthotizationCodeBLL.GenerateCode(request);
                return MyResult.OK(new { code, userName = request.MemberName });
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
        /// <summary>
        /// 激活授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("activeCode")]
        public MyResult ActiveCode(AuthotizationCodeRequest request)
        {
            try
            {
                request.MemberName = Request.Headers["username"];
                var res = AuthotizationCodeBLL.ActiveCode(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
        /// <summary>
        /// 使用授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("useCode")]
        public MyResult UseCode(AuthotizationCodeRequest request)
        {
            try
            {
                request.Status = "200";
                request.MemberName = Request.Headers["username"];
                var res = AuthotizationCodeBLL.UseCode(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
        /// <summary>
        /// 查找用户当前授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("getCodeInfo")]
        public MyResult GetCodeInfo()
        {
            try
            {
                string memberName = Request.Headers["username"];
                var res = AuthotizationCodeBLL.GetCodeInfosByUser(memberName).FirstOrDefault(c => c.Status == "200");
                if (res != null)
                {
                    AuthotizationCodeBLL.UpdateAuthotizationCode(res);
                    var res2 = AuthotizationCodeBLL.GetCodeInfosByUser(memberName).FirstOrDefault(c => c.Status == "200");
                    return MyResult.OK(res2);
                }
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
        /// <summary>
        /// 查找用户全部授权码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("getCodeInfosByUser")]
        public MyResult GetCodeInfosByUser()
        {
            try
            {
                string memberName = Request.Headers["username"];
                var res = AuthotizationCodeBLL.GetCodeInfosByUser(memberName);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
        /// <summary>
        /// 授权企业账户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("activeEnterpriseAccount")]
        public MyResult ActiveEnterpriseAccount(EnterpriseAccountRequest request)
        {
            try
            {
                var res = AuthotizationCodeBLL.ActiveEnterpriseAccount(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        /// <summary>
        /// 查找企业账户名下全部子账号
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("getSubAccountList")]
        public MyResult GetSubAccountList()
        {
            try
            {
                string username = Request.Headers["username"];
                var res = AuthotizationCodeBLL.GetSubAccountList(username);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        /// <summary>
        /// 授权全部子账户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("saveSubAccount")]
        public MyResult SaveSubAccount(SubAccountRequest request)
        {
            try
            {
                var res = AuthotizationCodeBLL.SetSubAccount(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
        /// <summary>
        /// 删除子账户
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("deleteSubAccount")]
        public MyResult DeleteSubAccount(SubAccountRequest request)
        {
            try
            {
                var res = AuthotizationCodeBLL.DeleteAuthotizationCode(request);
                return MyResult.OK(res);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
    }
}
