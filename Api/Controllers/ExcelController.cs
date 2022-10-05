using Api.BLL;
using Api.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    public class ExcelController : Controller
    {
        /// <summary>
        /// 查询平台基础数据（默认半个月）
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPlatformBusinessData")]
        public MyResult GetPlatformBusinessData()
        {
            var result = ExcelBLL.GetPlatformBusinessData();
            return MyResult.OK(result);
        }

        /// <summary>
        /// 导出平台基础数据（默认半个月）
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExportPlatformBusinessData")]
        public MemoryStream ExportPlatformBusinessData()
        {
            var bt = ExcelBLL.ExportPlatformBusinessData();
            MemoryStream ms = new MemoryStream(bt);
            return ms;
        }

        /// <summary>
        /// 导出平台基础汇总数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExportPlatformBusinessSummary")]
        public MemoryStream ExportPlatformBusinessSummary()
        {
            var bt = ExcelBLL.ExportPlatformBusinessData("summary");
            MemoryStream ms = new MemoryStream(bt);
            return ms;
        }

        /// <summary>
        /// 查询会员情况
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetMemberData")]
        public MyResult GetMemberData()
        {
            var result = ExcelBLL.GetMemberData();
            return MyResult.OK(result);
        }

        /// <summary>
        /// 导出会员情况
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExportMemberData")]
        public MemoryStream ExportMemberData()
        {
            var bt = ExcelBLL.ExportMemberData();
            MemoryStream ms = new MemoryStream(bt);
            return ms;
        }

        /// <summary>
        /// 查询商家返利情况
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProfitData")]
        public MyResult GetProfitData([FromQuery] ProfitDataRequest request)
        {
            var result = ExcelBLL.GetProfitData(request);
            return MyResult.OK(result);
        }

        /// <summary>
        /// 导出商家返利情况
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExportProfitData")]
        public MemoryStream ExportProfitData([FromQuery] ProfitDataRequest request)
        {
            var bt = ExcelBLL.ExportProfitData(request);
            MemoryStream ms = new MemoryStream(bt);
            return ms;
        }
    }
}
