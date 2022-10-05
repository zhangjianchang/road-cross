using Api.BLL;
using Api.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OriginalSystemController : ControllerBase
    {
        /// <summary>
        /// 同步用户表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SyncUsers")]
        public MyResult SyncUsers(string cookie)
        {
            if (ModelState.IsValid)
            {
                bool re = OriginalSystemBLL.SyncUsers(cookie);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }
        /// <summary>
        /// 同步柜机表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SyncCabinet")]
        public MyResult SyncCabinet(string cookie)
        {
            if (ModelState.IsValid)
            {
                bool re = OriginalSystemBLL.SyncCabinet(cookie);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }
        /// <summary>
        /// 同步柜机表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("SyncBottle")]
        public MyResult SyncBottle(string cookie, string page)
        {
            if (ModelState.IsValid)
            {
                bool re = OriginalSystemBLL.SyncBottle(cookie, page);
                return re ? MyResult.OK() : MyResult.Error();
            }
            return MyResult.Error();
        }
    }
}
