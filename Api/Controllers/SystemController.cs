using Api.BLL;
using Api.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        // GET: api/Home
        [HttpGet("apilog")]
        public MyResult ApiLog([FromQuery] ApiLogParam param)
        {
            int total;
            List<ApiLog> logList = SystemBLL.GetApiLogList(param, out total);

            var result = new
            {
                total,
                list = logList
            };
            return MyResult.OK(result);
        }
    }
}
