using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Api.BLL;
using Api.Entity;
using Api.Utilities;
using System;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignController : ControllerBase
    {
        [HttpPost("save")]
        public MyResult Save(SaveRequest request)
        {
            try
            {
                var token = Request.Headers["authorization"];
                //TODO后期需要统一验证token
                request.UserName = Request.Headers["userName"];
                var result = DesignBLL.Save(request);
                return MyResult.OK("保存成功", result);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        [HttpPost("delete")]
        public MyResult Delete(SaveRequest request)
        {
            try
            {
                var result = DesignBLL.Delete(request);
                return MyResult.OK("删除成功", result);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        [HttpPost("getByGuid")]
        public MyResult GetByGuid(SaveRequest request)
        {
            try
            {
                var result = DesignBLL.GetByGuid(request);
                return MyResult.OK("成功", result);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }

        [HttpPost("getList")]
        public MyResult GetList()
        {
            try
            {
                string UserName = Request.Headers["userName"];
                var result = DesignBLL.GetLIstByUser(UserName);
                return MyResult.OK("成功", result);
            }
            catch (Exception ex)
            {
                return MyResult.Error(ex.Message);
            }
        }
    }
}
